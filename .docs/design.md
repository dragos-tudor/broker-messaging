
## Design [v1]

## Main Flows
- inbound: consume and handle broker messages.
- outbound: publish broker messages.

## Transport
- conceptually there are inbound and outbound envelope types.
- inbound and outbound envelopes share the same envelope abstraction.
- envelopes and dead letter envelopes are interfaces.
- each broker-specific transport library implements two wrappers over its native broker message structure:
  - envelope wrapper.
  - dead letter envelope wrapper.
- pipelines show exactly the natural flow of information [excepting retry plan].
- envelopes stay at the pipeline edges:
  - for inbound pipeline at the boundary with the broker consumer.
  - for outbound pipeline at the boundary with the broker producer.

### Transport `Envelope`:
- envelopes are created by:
  - inbound pipeline wrapping and mapping broker specific messages.
  - outbound pipeline mapping outbox messages.
- envelope interface has 4 open generics: `<TKey, TValue, TMetadata, TConfirmation>`.
  - `TKey` the envelope `Key` type.
  - `TValue` the envelope `Value` type [usually `byte[]`].
  - `TMetadata` contains transport metadata required by the broker-specific envelope implementation.
  - `TConfirmation` is meaningful for inbound envelopes; outbound envelopes may use an empty/default confirmation representation.
- `Type` field contains envelope value type extracted from Metadata meaning the domain message type.
  - used by developers implemented handlers to distinguish how to deserialize inbox message `Payload`.
- the mappings between:
  - envelope `Value` → inbox message `Payload` should be implemented by developers.
  - outbox message `Payload` → envelope `Value` should also be implemented by developers.
- envelopes are transient transport structures and are never persisted directly by the core pipeline.

### Transport `DeadLetterEnvelope`
- dead letter envelopes are created by the inbound pipeline:
  - converting invalid envelopes.
  - mapping invalid inbox messages.
- dead letter envelope interface has 4 open generics `<TKey, TValue, TMetadata, TConfirmation>`:
  - `TKey` the originated envelope `Key` type.
  - `TValue` the originated envelope `Value` type [usually byte[]].
  - `TMetadata` the originated envelope `Metadata`.
  - `TConfirmation` the originated envelope `Confirmation`.
- `Type` the originated envelope `Type`.
- dead letter envelopes are transient transport structures and are never persisted directly by the core pipeline.

## Persistence
- persistent structures are classes.
- inbound pipelines process inbox messages and dead letter messages.
- inbound pipelines use retry plans.
- outbound pipelines process outbox messages.

### Persistence `InboxMessage`
- inbox messages are created by the inbound pipeline mapping envelopes.
- inbox message class has 2 open generics `<TKey, TPayload>`.
  - `TKey` the inbox message type mapped from envelope `TKey`.
  - `TPayload` the inbox message payload type [usually `byte[]` or JSON `string`].
- `Metadata` field should keep the JSON serialized envelope `Metadata`.
- inbox message statuses are: Initial, Processing, Handled, Abandoning, Closed.
  - before persistence inbox message status = Initial [in-memory status].
  - after persistence inbox message status = Processing.
- inbox message fields have constraints used for validation before persistence.
- fields constraints are enforced in parallel by:
 - dedicated validation functions.
 - data annotations.

### Persistence `DeadLetterMessage`
- dead letter messages are created by the inbound pipeline converting invalid inbox messages.
- dead letter message class has 2 open generics `<TKey, TPayload>`:
  - `TKey` the dead letter message type mapped from inbox message `TKey`.
  - `TPayload` the dead letter message payload mapped from inbox message `TPayload`.
- dead letter message statuses are: Processing, Published, Abandoned.

### Persistence `OutboxMessage`
- outbox messages are created by the user to publish them to brokers.
- outbox message class has 2 open generics `<TKey, TPayload>`:
  - `TKey` the outbox message type should be the same as the inbox message `TKey` (reasons in decisions.md).
  - `TPayload` the outbox message payload should be the same as the inbox message `TPayload`.
- outbox message statuses are: Processing, Published, Abandoned.
- outbox message fields have constraints used for validation before persistence.
- fields constraints are enforced in parallel by:
 - dedicated validation functions.
 - data annotations.

### Persistence `RetryPlan`
- retry plan is a durable recovery mechanism.
- retry plan mechanism is used for 2 structures:
  - non-persisted inbox messages.
  - dead letter envelope [non-persisted by design].
- retry plans mechanis is used ONLY and AFTER for operations failures:
  - first check the current retry plan for current structure.
  - when not exhausted pipeline route to register new retry plan.
- retry plans mechanism avoid poison messages limiting failed operation retries per entity.
- retry plan is not a message.

## Operations
- transport operations process transient transport structures and return explicit operation states.
- persistence operations process durable structures and return explicit operation states.
  - implement transactional inbox pattern.
  - implement transactional outbox pattern.
- each operation -> one-task responsibility (eg. capturing, inserting, handling, mapping).
- each operation is independent of the others.
- each operation wrap one main function.
- each operation follow one implementation pattern:
  - prepare the data.
  - invoke the main function.
  - analize function result [optional].
  - return state.
- each operation uses specialized interfaces for services and data based on composition root pattern.
- execution types:
  - `side-effects` operations [async].
  - `pure`, `side-effects-free` operations [sync].
- use try/catch blocks consistently [even for `sync` operations];
- all operations have the same signature:
  - input data + services + cancellation token as parameters.
  - (output data, state, exception?) as return type.

### Inbound operations
- **envelope**:
  - capture and validate the broker message into broker-agnostic envelope.
  - map envelope to an inbox message.
  - confirm the envelope to broker [using `Confirmation`].
  - convert invalid envelopes to dead-letter envelopes.
  - invalid envelopes are:
    - unrecoverable.
    - confirmable [`Confirmation` exists].

- **inbox**:
  - validate, insert, handle, transact, schedule retries, abandon, close inbox messages.
  - insertion could be idempotent [ensuring deduplication for at-least-once consuming strategy].
  - inbox messages retries statuses:
    - non-exhausted -> `Processing`.
    - exhausted -> `Abandoning`.
  - inbox messages statuses:
    - `Processing`: for inserting and non-exhausted scheduling.
    - `Abandoning`: for handling domain error and exhausted scheduling.
    - `Closed`: for transacting success.
  - convert invalid inbox messages to dead-letter messages.
  - check, register retry plans [non-persisted inbox message].

- **dead letter**:
  - insert, schedule retries, abandon, close dead letter messages.
  - insertion could be idempotent [for `Abandoning` inbox messages].
  - dead letter messages statuses:
    - `Processing`: inserting and non-exhausted scheduling.
    - `Published`: publishing/producing dead letter envelope success.
    - `Abandoned`: exhausted scheduling.

- **dead letter envelope**:
  - redirect ephemeral dead-letter envelopes.
  - publish persisted dead-letter envelopes.
  - produce persisted dead-letter envelopes.
  - produce callback success mark the related dead-letter message `Published`.
  - produce callback failures are instrumented.
  - check, register retry plans [dead letter envelopes = non-persistent structure].

### Outbound operations
- **outbox**:
  - validate, transact, map, schedule, abandon, close .
  - outbox messages statuses:
    - `Processing`: transacting and non-exhausted scheduling.
    - `Published`: publishing/producing [outbox] envelope success.
    - `Abandoned`: exhausted scheduling.

- **envelope**:
  - publish, produce outbound envelopes.
  - produce callback failures are instrumented.

## Inbound Pipeline
- inbound pipeline is composed from all 4 pipeline segments.
- inbound happy path: capturing -> validating -> mapping -> inserting -> confirming -> handling -> transacting -> closing.
- operation actions = connection mechanism: connect last operation -> next operation.
- one pipeline action could be:
  - prescriptive ["do this"].
  - descriptive ["this happened"].
  - descriptive ["terminal"].
- each pipeline segment implement:
  - define pipeline actions group.
  - define specialized services and data interfaces.
  - action -> operation = action mapper [`GetInboxPipelineOperation`].
  - operation -> action = pipeline mapper [`GetInboxPipelineAction`].
- all pipelines use shared `InboundPipelineData` operations service param.
- all pipelines use shared `InboundPipelineServices` operations service param.
- inbound pipeline:
  - define pipeline services interface composing all pipeline segments services interfaces.
  - define pipeline data interface composing all pipeline segments data interfaces.
  - action -> operation = combining segment action mappers [`GetInboundPipelineOperation`].
  - action -> action = cross-pipelines mapper [`MapInboundPipeline`].
  - operation -> action = combining segment pipeline mappers [`GetInboundPipelineAction`].
- `TerminalActions` signal pipelines terminal action.

## Design Vocabulary
- transport & persistence:
  - converting: transform structures from the same type group [eg. envelope -> dead letter envelope].
  - mapping: transform structures from different type groups [eg. envelope -> inbox message].
- verbs:
 - operation names use -ing forms: Mapping, Inserting, Handling
 - completed actions/states use completed forms: Mapped, Scheduled, Confirmed, Closed.