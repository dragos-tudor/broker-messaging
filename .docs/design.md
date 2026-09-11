
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
  - for inbound pipeline at the start boundary with the broker consumer for envelopes.
  - for inbound pipeline at the end boundary with the broker producer for dead letter envelopes.
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
  - envelope `TValue` → inbox message `TPayload` should be implemented by developers.
  - outbox message `TPayload` → envelope `TValue` should be implemented by developers.
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
- outbound pipelines process outbox messages.

### Persistence `InboxMessage`
- inbox messages are created by the inbound pipeline mapping envelopes.
- inbox message class has 2 open generics `<TKey, TPayload>`.
  - `TKey` the inbox message type mapped from envelope `TKey`.
  - `TPayload` the inbox message payload type [usually `byte[]` or JSON `string`].
- `Metadata` field should keep the JSON serialized envelope `Metadata`.
- inbox message statuses are: Processing [default], Handled, DeadLettering, Abandoned, Closed.
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
- use try/catch blocks wrappers consistently [even for `sync` operations];
- all operations have the same signature:
  - input data + services + cancellation token as parameters.
  - (output data, state, exception?) as return type.
- operations.* projects are organized by message type and direction.
- the repository is the source of truth for exact operation outcome states.

### Operations.Inbound.Envelope

* **Capturing** — obtains the next broker envelope.
* **Verifying** — performs lightweight envelope verification before mapping.
* **Mapping** — maps the inbound envelope into an `InboxMessage`.
* **Converting** — converts an inbound envelope failure into a `DeadLetterEnvelope`.
* **Confirming** — confirms the original broker envelope, including final confirmation when processing ends without entering Handling.

### Operations.Inbound.Inbox

* **Validating** — validates `InboxMessage` data.
* **Inserting** — persists the `InboxMessage`.
* **Handling** — invokes developer-provided business handling.
* **Transacting** — persists handling-side transactional changes.
* **Scheduling** — persists retry scheduling information for later handling.
* **DeadLettering** — hands the `InboxMessage` to dead-letter processing.
* **Converting** — converts the `InboxMessage` into a `DeadLetterMessage`.
* **Closing** — completes the original `InboxMessage` lifecycle.
* **Abandoning** — marks the `InboxMessage` as abandoned when processing cannot continue.

### Operations.Inbound.DeadLetter

* **Mapping** — maps a persisted `DeadLetterMessage` into a `DeadLetterEnvelope`.
* **Inserting** — persists a `DeadLetterMessage`.
* **Scheduling** — persists retry scheduling information for later publishing.
* **Closing** — completes the `DeadLetterMessage` lifecycle after successful publication.
* **Abandoning** — marks the `DeadLetterMessage` as abandoned when publishing cannot continue.

### Operations.Inbound.DeadLetterEnvelope

* **Redirecting** — publishes a `DeadLetterEnvelope` created from an inbound pre-persistence failure.
* **Publishing** — publishes a `DeadLetterEnvelope` synchronously.
* **Producing** — submits a `DeadLetterEnvelope` asynchronously and registers broker-result handling.
* **Dispatching** — processes the asynchronous broker produce result.

### Operations.Outbound.Outbox

* **Validating** — validates `OutboxMessage` data.
* **Transacting** — persists the `OutboxMessage` within the developer transaction.
* **Mapping** — maps a persisted `OutboxMessage` into an outbound `Envelope`.
* **Scheduling** — persists retry scheduling information for later publishing.
* **Closing** — completes the `OutboxMessage` lifecycle after successful publication.
* **Abandoning** — marks the `OutboxMessage` as abandoned when publishing cannot continue.

### Operations.Outbound.Envelope

* **Publishing** — publishes the outbound `Envelope` synchronously.
* **Producing** — submits the outbound `Envelope` asynchronously and registers broker-result handling.
* **Dispatching** — processes the asynchronous broker produce result.

## Pipelines
- operations produce outcomes; pipelines define semantic continuation from those outcomes.
- operation actions = connection mechanism: connect last operation -> next operation.
- one pipeline action could be:
  - prescriptive ["do this"].
  - descriptive ["terminal"].
- one pipeline = mapper between last operation state and next action.
- each pipeline segment implement:
  - pipeline actions group [eg. `CapturingActions`].
  - specialized services and data interfaces [eg. `ICapturingServices`, `ICapturingData`].
  - action -> operation = action mapper [eg. `GetCapturingOperation`].
  - operation -> action = pipeline mapper [eg. `MapCapturingAction`].

### Inbound Pipeline
- inbound pipeline is composed from 6 pipeline segments:
  - capturing.
  - redirecting.
  - handling.
  - deadlettering.
  - publishing.
  - dispatching.
- inbound happy path: capturing -> verifying -> mapping -> validating -> inserting -> confirming -> handling -> transacting.
- inbound pipeline:
  - define pipeline services interface composing all pipeline segments services interfaces [eg. `InboundPipelineServices`].
  - define pipeline data interface composing all pipeline segments data interfaces [eg. `InboundPipelineData`].
- `TerminalActions` signal pipelines terminal action.

### Resiliency `RetryPlan`
- retry plan is an in-memory recovery mechanism.
- retry plan mechanism is used for non-persisted structures [scheduling for persisted structures].
- retry plan mechanism is transparent for operations and is used at router level.

## Design Vocabulary
- transport & persistence:
  - converting: transform structures from the same type group [eg. envelope -> dead letter envelope].
  - mapping: transform structures from different type groups [eg. envelope -> inbox message].