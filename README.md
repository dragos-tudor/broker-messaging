## Broker agnostic messaging client library.

A broker-agnostic **transactional inbox/outbox** library for .NET, built
around durable orchestrated pipelines rather than ad-hoc retry logic
scattered through consumer/producer code. Ships as a base package
(`Messaging.Core`) plus thin broker-specific packages
(`Messaging.Kafka`, and RabbitMQ/Azure Service Bus following the same
seam - wip) — write your consuming/producing logic once, swap the broker
underneath without touching pipeline code.

(*ongoing design/architecture docs on [docs](./.docs)*).

---

## AI Models Used
- Design/Architecture sessions: Sonnet 5 (Thinking), GPT 4.5 Mini (Thinking) (web + live conversations).
- Implementation plan and code generation: Sonnet 5 (Medium) (Github Copilot).
- Implementation plan and tests generation: Gemini 3.7 Flash (Medium) (Google Antigravity VSCode extension).


## The core idea: an orchestrator, not a rigid state machine

At any given moment, moving a message through the library means answering
one of two genuinely different questions — and the architecture keeps
them permanently separate, never letting one leak into the other:

1. **"Given the shape of this data, which pipeline applies?"** — answered
   once, by the **Router**, by inspecting the data itself (is this an
   `Envelope`? An `InboxMessage` mid-processing? A message already marked
   for dead-lettering?).
2. **"Given what the last operation just returned, what runs next?"** —
   answered repeatedly, by whichever **Pipeline** the Router picked, by
   looking up the returned state string in a table.

```
   ┌─────────────┐   1. picks pipeline   ┌──────────────┐
   │   Router    │ ────────by data─────▶ │   Pipeline    │
   │(orchestrator)│                      │ (pure lookup) │
   │             │ ◀──2. picks operation─│               │
   └──────┬──────┘        by state        └──────────────┘
          │
          │ actually calls/awaits the operation, gets a fresh
          │ state back, and asks the same two questions again
          ▼
   ┌─────────────┐
   │  Operation  │   a small, self-contained async function
   └─────────────┘
```

The distinction that matters: this is **not** just a state table being
replayed. The **Router is the orchestrator** — it's the only place
execution actually happens, and it carries real responsibilities beyond
"look up the next step": retry budgets and circuit-breaking, resolving
config-dependent branches a pipeline has no business deciding (Kafka
producer vs. AMQP publisher), and stitching together the hand-offs
*between* pipelines when one workflow's exit feeds another's entry.
Pipelines, by contrast, are deliberately inert — pure `state → operation`
lookup tables, one file, no execution, no memory of anything that
happened before the current state. Keeping the orchestrator "smart" and
every pipeline "dumb" is what makes each pipeline readable top-to-bottom,
and what closed an entire bug class the earlier design had: states
silently falling through because a pipeline's own internal loop and the
router disagreed about who was actually driving.

---

## Small operations, one job, uniform shape

Every operation in the library — `ValidateEnvelope`, `InsertInboxMessage`,
`ConfirmEnvelope`, all ~30 of them — has the identical signature:

```csharp
  internal static async ValueTask<(TData, string, Exception?)> InsertInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
```

Data in, data out, a state string describing what happened, and the
exception if any. This uniformity is what lets the orchestrator treat
*any* operation in the library identically through one delegate shape —
no special-casing per operation, no operation that needs to know what
kind of thing comes after it.

Two disciplines every operation follows:

- **Pure vs. side-effect, decided up front.** Pure operations (dev-supplied
  transforms like `Mapping`/`Converting`, marked non-throwing-by-contract)
  never self-loop on failure — retrying a pure function changes nothing,
  so their errors route straight to `Unrecoverable`. Side-effect
  operations (DB writes, broker calls) self-loop under a budget, because
  a transient failure might genuinely succeed next time.
- **Guard clauses, not defensive sprawl.** Each operation starts with a
  `Require*` call that asserts its own invariants and throws
  `InvalidOperationException` on violation — these are internal
  contracts, not external input validation, and they keep every function
  body reading as "the happy path, with the impossible cases fenced off
  at the top."

---

## Pipelines: readable, one-file lookup tables

Here's a real one — the entire dispatch logic for the inbox path, retry
mechanism included, in one switch statement:

```csharp
internal static InboxOperation InboxPipeline(string state) => state switch
{
    ValidateInboxMessageSuccessState => InboxOperation.CheckingRetry,
    InsertInboxMessageSuccessState  => InboxOperation.Handling,
    HandleInboxMessageSuccessState  => InboxOperation.Transacting,
    HandleInboxMessageDomainErrorState => InboxOperation.Abandoning,
    TransactInboxMessageSuccessState   => InboxOperation.Exit,
    // ...
    _ => InboxOperation.Unknown
};
```

No hidden control flow, no operation that knows what comes after it —
sequencing lives entirely in the table, which means the *whole shape* of
a workflow (inbox handling, dead-lettering, outbox dispatch) is visible
in one screen, not spread across the bodies of a dozen functions.

**Not every transition is a pure 1:1 lookup**, and rather than bolt a
general mechanism onto the pipeline for the exceptions, each is resolved
explicitly, in place, by the orchestrator — e.g. `Unrecoverable`/`Exit`
are deliberately **non-dispatchable**: the pipeline names the transition
but returns `null` for the delegate, forcing the Router to branch on it
explicitly rather than call through blindly. The same pattern extends to
genuinely cross-cutting decisions the pipeline table has no business
making — like picking between a Kafka producer and an AMQP publisher —
by naming a `Sending` state that the Router resolves against config,
rather than smuggling a config parameter into an otherwise pure lookup
function.

---

## Project layout: four layers, one direction of dependency

```
Layer 4 — Routing        Routing.Inbound / Routing.Outbound
                          (the orchestrator; owns retry budgets,
                           circuit-breaking, config branches,
                           cross-pipeline hand-offs)
                                        │
Layer 3 — Pipelines      Pipelines.Inbound / Pipelines.Outbound
                          (pure lookup tables, one folder per
                           mirrored Operations project)
                                        │
Layer 2 — Operations     six projects, split by DIRECTION × INPUT ENTITY:
                          Operations.Inbound.Envelope
                          Operations.Inbound.Inbox
                          Operations.Inbound.DeadLetter
                          Operations.Inbound.DeadLetterEnvelope
                          Operations.Outbound.Outbox
                          Operations.Outbound.Envelope
                                        │
Layer 1 — Persistence     / .Inbox / .Outbox / .DeadLetter / .RetryMessage
          Transport       / .Envelope / .DeadLetterEnvelope
```

Each layer references only the layer directly below it — a strict DAG,
no exceptions. The Operations split (six projects, not three, not five)
is the result of applying one rule consistently: **an operation lives in
the project named for what it *consumes*, crossed with which direction
it belongs to** — the same rule the library already uses for naming
individual operations (`ConvertEnvelope` takes an `Envelope`; it lives in
the Envelope project). This single rule resolved every awkward case —
`Mapping`/`Converting` operations that straddle two entity types,
`Envelope`-shaped operations that differ completely between inbound and
outbound — without ever needing a special "boundary operations" bucket.

Pipelines mirror this at folder granularity: `Pipelines.Inbound` has one
folder per Operations project it dispatches into, so navigating from "I'm
looking at `Operations.Inbound.Inbox`" to "here's its pipeline" is always
a one-hop, same-name lookup.

---

## Retry and exhaustion: one generic mechanism, three consequences

Rather than every operation carrying its own bespoke retry counter, the
orchestrator owns **one generic attempt-budget mechanism**, applied
uniformly to every side-effect operation's error state. What varies is
only the *consequence* once that budget is spent — and that consequence
falls into exactly three categories, decided by asking one question per
operation: *is there a legitimate next step once retries are exhausted,
or not?*

| Category | Examples | On exhaustion |
|---|---|---|
| Infra-only failures | `Closing`, `Abandoning` | Circuit opens, resumes same action later |
| Row already persisted | `Handling`, `Transacting` | Hands off to `Scheduling` (persisted retry) |
| No durable row yet | `Inserting`, `Publishing`/`Producing`/`Redirecting` | New `RetryMessage` mechanism (below) |
| No escape at all | `Capturing`, `Confirming` | Circuit-opens forever — by design, not oversight |

For the "no durable row yet" case — a message that can fail before
anything's been persisted to retry against — a small, dedicated
`RetryMessage` mechanism kicks in: a pre-check gate (`Checking`) runs
*before* the risky operation is even attempted, and an `Upserting` step
writes a durable retry record only once that operation's own budget is
truly spent. This record is deliberately **not a message store** — the
envelope is always re-fetched fresh from the broker on retry, never
deserialized from this table — which sidesteps an entire category of
staleness/versioning problems, and doubles as a genuine, queryable audit
trail of poison messages as a side benefit.


### Remarks
---
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
- gemini model discovered with almost 0-guidance the testing patterns and 95% for tests scenarios.