
## Design [v1]

## Main Flows

- inbound: consume and handle broker messages.
- outbound: publish broker messages.

## Transport

- transport structures:
  - envelope [`IEnvelope`].
  - dead letter envelope [`IDeadLetterEnvelope`].
- conceptually there are:
  - inbound envelopes.
  - inbound dead letter envelopes.
  - outbound envelopes.
- inbound and outbound envelopes share the same envelope abstraction.
- each broker-specific transport library implements interfaces as wrappers over its native broker message structure:
  - envelope wrapper [`Envelope`].
  - dead letter envelope wrapper [`DeadLetterEnvelope`].

### Transport.Envelope

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

### Transport.DeadLetterEnvelope

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

- persistence structures:
  - inbox message [`IInboxMessage`].
  - dead letter message [`IDeadLetterMessage`].
  - outbox message [`IOutboxMessage`].

### Persistence.InboxMessage

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

### Persistence.DeadLetterMessage

- dead letter messages are created by the inbound pipeline converting invalid inbox messages.
- dead letter message class has 2 open generics `<TKey, TPayload>`:
  - `TKey` the dead letter message type mapped from inbox message `TKey`.
  - `TPayload` the dead letter message payload mapped from inbox message `TPayload`.
- dead letter message statuses are: Processing, Published, Abandoned.

### Persistence.OutboxMessage

- outbox messages are created by the user to publish them to brokers.
- outbox message class has 2 open generics `<TKey, TPayload>`:
  - `TKey` the outbox message type should be the same as the inbox message `TKey`.
  - `TPayload` the outbox message payload should be the same as the inbox message `TPayload`.
- outbox message statuses are: Processing, Published, Abandoned.
- outbox message fields have constraints used for validation before persistence.
- fields constraints are enforced in parallel by:
 - dedicated validation functions.
 - data annotations.

## Operations

- transport operations process transient transport structures and return explicit operation states.
- persistence operations process persistent structures and return explicit operation
states.

### Operations.Inbound.Envelope

- capturing — obtains the next broker envelope.
- verifying — performs lightweight envelope verification before mapping.
- mapping — maps the inbound envelope into an `InboxMessage`.
- converting — converts an inbound envelope failure into a `DeadLetterEnvelope`.
- confirming — confirms the original broker envelope, including final confirmation when processing ends without entering Handling.

### Operations.Inbound.Inbox

- validating — validates `InboxMessage` data.
- inserting — persists the `InboxMessage`.
- handling — invokes developer-provided business handling.
- transacting — persists handling-side transactional changes.
- scheduling — persists retry scheduling information for later handling.
- deadlettering — hands the `InboxMessage` to dead-letter processing.
- converting — converts the `InboxMessage` into a `DeadLetterMessage`.
- closing — completes the original `InboxMessage` lifecycle.
- abandoning — marks the `InboxMessage` as abandoned when processing cannot continue.

### Operations.Inbound.DeadLetter

- mapping — maps a persisted `DeadLetterMessage` into a `DeadLetterEnvelope`.
- inserting — persists a `DeadLetterMessage`.
- scheduling — persists retry scheduling information for later publishing.
- closing — completes the `DeadLetterMessage` lifecycle after successful publication.
- abandoning — marks the `DeadLetterMessage` as abandoned when publishing cannot continue.

### Operations.Inbound.DeadLetterEnvelope

- redirecting — publishes a `DeadLetterEnvelope` created from an inbound pre-persistence failure.
- publishing — publishes a `DeadLetterEnvelope` synchronously.
- producing — submits a `DeadLetterEnvelope` asynchronously and registers broker-result handling.
- dispatching — processes the asynchronous broker produce result.

### Operations.Outbound.Outbox

- validating — validates `OutboxMessage` data.
- transacting — persists the `OutboxMessage` within the developer transaction.
- mapping — maps a persisted `OutboxMessage` into an outbound `Envelope`.
- scheduling — persists retry scheduling information for later publishing.
- closing — completes the `OutboxMessage` lifecycle after successful publication.
- abandoning — marks the `OutboxMessage` as abandoned when publishing cannot continue.

### Operations.Outbound.Envelope

- publishing — publishes the outbound `Envelope` synchronously.
- producing — submits the outbound `Envelope` asynchronously and registers broker-result handling [`ProduceResult`].
- dispatching — processes the asynchronous broker produce result.

### Operations Design Rules

- each operation has one-task responsibility [eg. capture an envelope, map a dead-letter message, validate an envelope, insert an inbox message].
- each operation is independent of the others.
- each operation uses specialized interfaces for services and data based on composition root pattern.
- all operations have similar signature:
  - services, shared data, cancellation token as parameters.
  - (output data, state, exception?) as return type.
- operations expected failures must return explicit states.
- operations mutate only their owned pipeline data.
- operations error handling is delegated to a centralized router function.
- use try/catch blocks wrappers consistently [even for `sync` operations];
- operations.* projects are organized by direction and structure type.

## Pipelines

Pipelines define semantic processing flow by mapping operation outcomes to the next action, another pipeline, or a terminal continuation.

### Pipelines.Inbound

- capturing: receives a broker envelope and drives it through `IEnvelope` verification, mapping, `InboxMessage` validation, persistence, and confirmation.
- redirecting: handles inbound failures that occur before a durable `InboxMessage` can continue processing.
- handling: processes a persisted `InboxMessage`.
- dead-lettering: converts a persisted `InboxMessage` into a persisted `DeadLetterMessage`.
- publishing: publishes a persisted `DeadLetterMessage`.
- dispatching: processes asynchronous broker produce results for `IDeadLetterEnvelope` publishing.

### Pipelines.Outbound

- persisting: validates and persists a developer-created `OutboxMessage`.
- publishing: publishes a persisted `OutboxMessage`.
- dispatching: processes asynchronous broker produce results for outbound `IEnvelope` publishing.

### Pipeline Design Rules

- pipeline maps an operation outcome to its semantic continuation.
- pipeline continuation may be:
  - another action in the same pipeline;
  - another pipeline;
  - `Exit`;
  - `Unrecoverable`.
- `Exit` means the current router/pipeline invocation has no further continuation; it does not necessarily mean the overall message lifecycle is finished.
- pipelines must not inspect message internals to infer control flow; continuation is determined from explicit operation outcomes.
- pipeline and operation identifiers are unique within their owning component; exact outcome-state names remain source-code implementation details.\
- each pipeline must define its entry action by mapping it to the first action. Subsequent mappings are driven by operation outcomes.

### Exception propagation
- each [almost] pipeline segment owns its specific error-propagation rules.
- operations success paths skip propagation entirely.
- propagation functions handle only error outcomes.
- router calls only the propagation function required by the current pipeline segment.
- structures mappers/converters transfer existing FailureReason themselves.
- `OutboxMessage` no longer carries FailureReason.


## Design Vocabulary
- structures:
  - converting: transform structures from the same type group [eg. envelope -> dead letter envelope].
  - mapping: transform structures from different type groups [eg. envelope -> inbox message].
- structures:
  - failure reason:
    - keep verifying, mapping, validating, handling, transacting errors for envelopes and inbox/outbox messages.
  - last error:
    - keep [repeatable] scheduled operations errors. sometimes could be the same as failure reason.
- operations:
  - verifying: performs lightweight envelope verification.
  - validating: performs heavyweight inbox and outbox messages validations [data annotations and specialized functions].

-