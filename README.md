## Broker agnostic messaging client library.

A high-throughput, **transactional inbox/outbox** messaging library as broker-backed systems built on .NET 10. Designed around a strict four-layer directed acyclic graph (DAG), uniform operation shapes, and an orchestrator pattern that keeps state machines pure and readable.

(*ongoing design/architecture docs on [docs](./.docs)*).

---

## AI Models Used
- Design/Architecture sessions: Sonnet 5 (Thinking), GPT-5.6 Luna (Thinking) (web + live conversations).
- Implementation plan and code generation: Sonnet 5 (Medium) (Github Copilot).
- Implementation plan and tests generation: Gemini 3.7 Flash (Medium) (Google Antigravity VSCode extension).

---

## The core idea: an orchestrator, not a rigid state machine

At any given moment, moving a message through the library means answering one of two genuinely different questions — and the architecture keeps them permanently separate:

1. **"Given the current state of this workflow, what runs next?"** — answered by a **Pipeline**, which is a pure, inert lookup table mapping operation state strings to scoped action strings (`state → action`).
2. **"Given what just happened across pipeline boundaries or in this data, how do we orchestrate it?"** — answered by the **Router**, which acts as the orchestrator.

```
   ┌──────────────┐   1. evaluates state   ┌───────────────┐
   │    Router    │ ─────────────────────▶ │   Pipeline    │
   │(orchestrator)│                        │ (pure lookup) │
   │              │ ◀──2. returns action───│ (state→action)│
   └──────┬───────┘                        └───────────────┘
          │
          │ 3. if action crosses pipelines:
          │    calls MapInboundAction(action, config)
          │
          │ 4. dispatches operation delegate, gets fresh state
          ▼
   ┌──────────────┐
   │  Operation   │   a small, self-contained async function
   └──────────────┘
```

The distinction that matters: this is **not** a monolithic state machine replayed blindly. The **Router is the orchestrator** — it is the only place asynchronous execution and side effects happen. It carries responsibilities that pipelines have no business deciding: retry budgets, circuit-breaking, DI/configuration seams (AMQP publisher vs. Kafka producer), and stitching together hand-offs *between* pipelines.

Pipelines, by contrast, are deliberately inert — pure `state → action` lookup tables, one file each, zero side effects, no memory of prior states.

---

## Small operations, one job, uniform shape

Every operation across the library — `ValidateEnvelope`, `InsertInboxMessage`, `ConfirmEnvelope`, `PublishDeadLetterEnvelope` — follows the exact same signature:

```csharp
internal static async ValueTask<(TData, string, Exception?)> InsertInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
```

Data in, mutated data out, a string constant describing the resulting state, and an exception if one occurred. This uniformity allows the orchestrator to resolve and dispatch *any* operation in the library through one delegate signature.

Two strict disciplines every operation follows:

- **Pure vs. side-effect, decided up front.** Pure operations (dev-supplied transforms like `Mapping` or `Converting`) never self-loop on failure — retrying an in-memory transform changes nothing, so errors route straight to `Unrecoverable`. Side-effect operations (database writes, broker network calls) self-loop under a router budget or retry mechanism, because a transient failure may succeed next time.
- **Guard clauses, not defensive sprawl.** Each operation begins with a `Require*` assertion that validates internal invariants (`RequireEnvelope`, `RequireInboxMessage`) and throws `InvalidOperationException` on violation. These represent internal contractual fences, keeping the body of the function focused purely on the happy path.

---

## Pipelines: readable, one-file lookup tables

Each pipeline is a single, zero-allocation switch statement. Actions are modeled not as boxed enums, but as **unique, compile-time interpolated constant strings** scoped by domain (e.g. `Inbox.Inserting`, `Envelope.Confirming`):

```csharp
internal static string InboxPipeline(string state) => state switch
{
  ValidateInboxMessageSuccessState => InboxActions.Inserting,
  ValidateInboxMessageErrorState => InboxActions.Unrecoverable,

  // Inserting — durability achieved on success; ANY failure routes to retry check
  InsertInboxMessageSuccessState => InboxActions.Inserted,
  // ...
  _ => InboxActions.Unknown
};
```

## Cross-pipeline orchestration: the mapping function

When an operation produces an action that crosses into another pipeline, it flows through a central, pure mapping function (`MapInboundAction`).

Organized along the natural lifecycle flow:

```csharp
partial class InboundFuncs
{
  internal static string? MapInboundAction(string action, InboundPipelineConfig config = default) =>
    action switch
    {
      // 1. Envelope Pipeline actions
      EnvelopeActions.Mapped => InboxActions.Validating,
      EnvelopeActions.Converted => EphemeralDeadLetterEnvelopeActions.Redirecting,

      ...
      // Terminal, deferred, and non-dispatchable actions
      _ => default
    };
}
```

### 1. Durability-Driven Sequential Confirmation

Offset confirmation (`ConfirmEnvelope`) does not happen at the tail end of all business logic. Instead, **confirmation happens at the exact moment durability is established or safely given up on**:

$$\text{Inserting} \xrightarrow{\text{Inserted}} \text{Confirming} \xrightarrow{\text{Confirmed}} \text{Handling} \xrightarrow{\text{Handled}} \text{Transacting}$$

1. **`Inbox.Inserting` succeeds** $\to$ emits `InboxActions.Inserted`.
2. **`MapInboundAction`** maps `Inserted` to `EnvelopeActions.Confirming`.
3. **`ConfirmEnvelope` runs** $\to$ broker offset is durably committed $\to$ emits `EnvelopeActions.Confirmed`.
4. **`MapInboundAction`** maps `Confirmed` to `InboxActions.Handling`.
5. **The Router Guard**: The Router checks `data.InboxMessage?.Status == InboxMessageStatus.Processing`:
   - For `Inserted`: message is in `Processing` $\to$ dispatches `Handling`.
   - For `Idempotent` (duplicate cleared), `RetryExhausted` (insert failed), or `Redirected` (malformed payload): the condition is false $\to$ the Router terminates cleanly without dispatching `Handling`.

Downstream business execution (`Handling`, `Transacting`, `Abandoning`, `Scheduling`) operates on an already-committed database row and never touches broker confirmation again.


### 2. Decoupled Dead Letter Paths: Ephemeral vs. Persisted

Dead lettering is split into two mutually exclusive paths:

- **Ephemeral Path (`EphemeralDeadLetterEnvelopePipeline`)**:
  Used when validation or mapping fails on the inbound envelope (`EnvelopeActions.Converted`). No database row exists. It attempts an in-memory redirect. Under failure, it follows pre-durable retries (`CheckingRetry` $\to$ `UpsertingRetry` $\to$ `Deferring`). On success or exhaustion (`Redirected` / `CheckedRetry`), it maps directly to `EnvelopeActions.Confirming`.
- **Persisted Path (`DeadLetterEnvelopePipeline`)**:
  Used when an inserted inbox message encounters a fatal business domain error (`InboxActions.Converted` $\to$ `DeadLetterActions.Inserting`). A durable `DeadLetterMessage` database row exists. `DeadLetterActions.Mapped` dynamically routes to `Publishing` or `Producing` via `InboundPipelineConfig`. On `Published`, it hands off to `DeadLetterActions.Closing`, which in turn triggers `InboxActions.Closing` to close both database records in a clean two-way roundtrip.

---

## Project layout: four layers, one direction of dependency

```
Layer 4 — Routing        Routing.Inbound / Routing.Outbound
                          (the orchestrator; owns retry budgets,
                           circuit-breaking, config branches,
                           cross-pipeline hand-offs)
                                        │
Layer 3 — Pipelines      Pipelines.Inbound / Pipelines.Outbound
                          (pure lookup tables, MapInboundAction,
                           and pipeline configuration)
                                        │
Layer 2 — Operations     split by DIRECTION × INPUT ENTITY:
                          Operations.Inbound.Envelope
                          Operations.Inbound.Inbox
                          Operations.Inbound.DeadLetter
                          Operations.Inbound.DeadLetterEnvelope
                          Operations.Outbound.Outbox
                          Operations.Outbound.Envelope
                                        │
Layer 1 — Persistence     / .InboxMessage / .OutboxMessage
                          / .DeadLetterMessage / .RetryMessage
          Transport       / .Envelope / .DeadLetterEnvelope
```

Each layer references only the layer directly below it — a strict DAG. An operation lives in the project named for what it **consumes**, crossed with the **direction** it belongs to (`ConvertEnvelope` consumes an `Envelope` on inbound $\to$ `Operations.Inbound.Envelope`).

`Pipelines.Inbound` mirrors this at folder granularity (`Envelope`, `Inbox`, `DeadLetter`, `DeadLetterEnvelope`) alongside the centralized `Mapping.cs`.

---

## Retry and exhaustion: check-after-error and unified budgets

Rather than each operation maintaining custom retry logic, retry orchestration follows two distinct models based on whether durability has been reached:

| Category | Examples | Lifecycle Strategy |
|---|---|---|
| **Infra / Cleanup Operations** | `Closing`, `Abandoning` | Router-generic attempt budget; circuit opens on repeated failure. |
| **Row Already Persisted** | `Handling`, `Transacting` | Router budget; upon exhaustion, hands off to `Scheduling` (database retry worker). |
| **Pre-Durable Write Operations** | `Inserting`, `Redirecting` | Dedicated `RetryMessage` audit check-after-error mechanism (below). |

### The Check-After-Error Pre-Durable Retry (`RetryMessage`)

For messages that have not yet achieved durability (e.g. `Inserting` an `InboxMessage` or `Redirecting` an ephemeral dead-letter envelope):

1. The initial operation is attempted directly without pre-check overhead.
2. If an error occurs (`InsertInboxMessageErrorState`), control passes to `CheckingRetry`.
3. `CheckingRetry` inspects the durable `RetryMessage` store:
   - **Not Exhausted**: routes to `UpsertingRetry`, records the attempt, and returns `Deferring`. The message is not confirmed; the broker will redeliver it after a backoff delay.
   - **Exhausted**: routes to `RetryExhausted`, hands off to `EnvelopeActions.Confirming`. The poison envelope offset is committed so the broker partition does not stall, and `Handling` is skipped.

The `RetryMessage` record is **not a message store** — the payload is always re-read fresh from the broker upon redelivery, preventing payload version skew while providing a fully queryable audit trail of poison message attempts.

---

## Testing Strategy: Two-Tier Verification

The pipeline and mapping architecture is tested across two complementary tiers using composite interfaces (`IInboundServices`, `IInboundData`):

1. **Tier 1: Fast Graph Traversal Tests (Zero Mocks, <10ms)**: Validates pure string sequences (`Pipeline(state) → MapInboundAction(action)`) across every permutation of branches and error paths.
2. **Tier 2: Async Scenario Runner Tests (NSubstitute Mocks)**: Executes the actual async operation delegates through `RunInboundPipelineAsync` to verify end-to-end data mutations, offset confirmation order, idempotency clearing, and status guards across all 5 canonical lifecycle scenarios:
   - **Happy Path**: `Envelope.Mapped` $\to$ `Inbox.Inserted` $\to$ `Envelope.Confirmed` $\to$ `Inbox.Handled` $\to$ `Inbox.Transacted`.
   - **Idempotent Duplicate**: Duplicate insert returns false $\to$ `Inbox.Idempotent` $\to$ `Envelope.Confirmed` $\to$ Router guard stops cleanly without `Handling`.
   - **Insert Failure Exhaustion**: Insert error $\to$ `CheckingRetry` exhausts $\to$ `Envelope.Confirmed` $\to$ Router guard stops cleanly.
   - **Malformed Envelope**: `Envelope.Converted` $\to$ `EphemeralDL.Redirecting` $\to$ `Envelope.Confirmed`.
   - **Persisted Dead-Letter Two-Way Crossing**: Domain error $\to$ `Inbox.Converted` $\to$ `DeadLetter.Inserting` $\to$ `DeadLetter.Mapped` $\to$ `DLEnvelope.Published` $\to$ `DeadLetter.Closed` $\to$ `Inbox.Closed`.

---

## Remarks
- all integration tests use podman containers [aspire testing NA].
- dev container network is user-created. ensure isolation from host [messaging-netwok].
- podman containers are isolated using dedicated network [dev-netwok].
- podman containers:
  - when dev container is created podman containers are created.
  - when dev container is started podman containers are started (avoiding ghosts ports hanging).
  - when any, podman pull images from host registry images container.
  - coredns is using to resolve the kafka containers names inside containers network and from dev container.
- functional-style library [OOP-free].
- podman-inside-of-podman.
