# Broker-Messaging — Handoff (2026-08-31, Inbox/DeadLetterEnvelope retry
   mechanism reworked for performance; Router-level batching idea captured)

Continuation handoff. Load alongside
`restructuring-pipelines-projects-handoff-2026-08-26.md`,
`inbound-pipelines-handoff-2026-08-28.md`,
`inbound-pipelines-retry-mechanism-handoff-2026-08-30.md`, and
`inbound-pipelines-finalized-handoff-2026-08-30.md`. This doc
**supersedes** 08-30 §3 (`Pipelines.Inbound.Inbox`) and §5
(`Pipelines.Inbound.DeadLetterEnvelope`) — the retry-mechanism shape for
both changed today, motivated by a real performance problem, not a
stylistic preference. As always: code wins if it contradicts this doc.

---

## 1. Why this changed: the pre-check was costing a DB round trip per healthy message

The 08-29 design ran `CheckingRetry` as a **pre-check gate**, before
`Inserting`/`Redirecting`/`Publishing`/`Producing` were ever attempted —
meaning every single message, healthy or not, paid a DB round trip to ask
"has this been given up on before?" before doing the actual work. Given
`RetryMessage` rows only ever exist for genuinely poisoned messages (rare
by construction), that pre-check was buying almost nothing for the
overwhelming majority of traffic while directly gating how fast the
happy path could write.

**The fix, settled today: check only after the guarded operation itself
fails**, not before. A healthy message now costs exactly one DB round
trip total for its guarded operation (the operation itself) — no
Checking before, no Checking after success. Only a message that's
*already failing* pays the extra Check+Upsert pair, and only once per
delivery attempt.

---

## 2. New shared vocabulary: `Deferring`

Both reworked pipelines needed a way to express a destination that isn't
`Exit` (→ confirm/advance) and isn't `Unrecoverable` (→ dev-contract
violation) — specifically: **"not exhausted yet, stop here, do not
confirm, let the broker's own redelivery bring this message back
later."** This is deliberately distinct from `Unknown` — `Unknown` means
"this state string matched nothing in the table, something is actually
broken"; `Deferring` means "this is the correct, expected outcome of a
message that's mid-retry." Collapsing the two would make Router logs
unable to distinguish a real bug from healthy backpressure — kept
separate on purpose.

`Deferring` is non-dispatchable, same family as `Exit`/`Unrecoverable` —
`Get*Operation` returns `default` for it, and the Router simply does
nothing further this pass.

**Operational consequence worth remembering:** because offset genuinely
doesn't advance while `Deferring`, every message behind a
consistently-failing one in the same partition is blocked until it
either succeeds or exhausts. That's correct for the pre-durable case
(nothing's safely persisted yet, so nothing can be skipped past), but a
sustained failure now shows up as a partition-wide stall rather than a
per-message delay — a real operational characteristic, not a bug to fix
later.

---

## 3. `Pipelines.Inbound.Inbox` — reworked (supersedes 08-30 §3)

```csharp
internal enum InboxOperation
{
    Validating,
    Inserting,
    CheckingRetry,
    UpsertingRetry,
    Handling,
    Transacting,
    Abandoning,
    Scheduling,
    Converting,
    Closing,
    Unrecoverable,
    Deferring,
    Exit,
    Unknown
}
```

```csharp
internal static InboxOperation InboxPipeline(string state) => state switch
{
    // Validating (pure — no self-loop on either failure)
    ValidateInboxMessageSuccessState => InboxOperation.Inserting,        // straight to Inserting, no pre-check gate
    ValidateInboxMessageErrorState => InboxOperation.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxOperation.Unrecoverable,

    // Inserting (side effect — no self-loop; ANY failure routes to CheckingRetry)
    InsertInboxMessageSuccessState => InboxOperation.Handling,           // transition Status: Mapping → Processing
    InsertInboxMessageErrorState => InboxOperation.CheckingRetry,
    IdempotentInboxMessageState => InboxOperation.Exit,                  // → ConfirmEnvelope, bypassing Converting

    // CheckingRetry (side effect — runs only after an Insert error; already known-exhausted from a prior session?)
    CheckRetryInboxMessageExhaustedState => InboxOperation.Exit,         // already given up previously — Confirming
    CheckRetryInboxMessageNotExhaustedState => InboxOperation.UpsertingRetry,
    CheckRetryInboxMessageErrorState => InboxOperation.CheckingRetry,    // self-loop, infra failure on the check itself

    // UpsertingRetry (side effect — records this attempt, decides exhaustion itself, same idiom as
    // ScheduleInboxMessageAsync's Retry/Exhausted split)
    UpsertRetryInboxMessageExhaustedState => InboxOperation.Exit,        // budget spent — Confirming, give up
    UpsertRetryInboxMessageRetryState => InboxOperation.Deferring,       // not yet exhausted — wait for broker redelivery
    UpsertRetryInboxMessageErrorState => InboxOperation.UpsertingRetry,  // self-loop, infra failure on the write itself

    // Handling (side effect — plain try/catch only; Router-generic budget governs self-loop + exhaustion;
    // exhaustion → Scheduling handoff lives in the Router's mapper table, not this table)
    HandleInboxMessageSuccessState => InboxOperation.Transacting,
    HandleInboxMessageDomainErrorState => InboxOperation.Abandoning,
    HandleInboxMessageErrorState => InboxOperation.Handling,

    // Transacting (side effect — same category as Handling; exhaustion handoff also Router-level)
    TransactInboxMessageSuccessState => InboxOperation.Exit,             // terminal, Status = Handled
    TransactInboxMessageErrorState => InboxOperation.Transacting,

    // Abandoning (side effect — pure infra failure, self-loop + Router circuit-open-and-resume)
    AbandonInboxMessageSuccessState => InboxOperation.Converting,
    AbandonInboxMessageErrorState => InboxOperation.Abandoning,

    // Scheduling (persisted retry, reached only via Router's mapper table from Handling/Transacting exhaustion)
    ScheduleInboxMessageExhaustedState => InboxOperation.Abandoning,
    ScheduleInboxMessageRetryState => InboxOperation.Exit,               // persisted, next job iteration picks it up
    ScheduleInboxMessageErrorState => InboxOperation.Scheduling,

    // Converting (pure — no self-loop on failure)
    ConvertInboxMessageSuccessState => InboxOperation.Exit,              // → DeadLetterMessage populated, hand off to Pipelines.Inbound.DeadLetter
    ConvertInboxMessageErrorState => InboxOperation.Unrecoverable,

    // Closing (side effect) — also reached via cross-pipeline hand-off from DeadLetterEnvelope's Exit
    // (when a DeadLetterMessage exists), not resolved by anything in this table
    CloseInboxMessageSuccessState => InboxOperation.Exit,                // terminal, Status = Closed
    CloseInboxMessageErrorState => InboxOperation.Closing,

    _ => InboxOperation.Unknown
};
```

```csharp
internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetInboxOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(InboxOperation action)
    where TServices : IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
    =>
    action switch
    {
        InboxOperation.Validating => ValidateInboxMessage<TServices, TData, TKey, TPayload>,
        InboxOperation.Inserting => InsertInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxOperation.CheckingRetry => CheckRetryInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxOperation.UpsertingRetry => UpsertRetryInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxOperation.Handling => HandleInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxOperation.Transacting => TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>,
        InboxOperation.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxOperation.Scheduling => ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxOperation.Converting => ConvertInboxMessage<TServices, TData, TKey, TPayload>,
        InboxOperation.Closing => CloseInboxMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
    };
```

Naming note: state strings and resolver method names dropped the
`For...` suffix (`CheckRetryInboxMessageAsync`, not
`CheckRetryInboxMessageForInsertingAsync`) — with only one guarded
operation in this pipeline, the disambiguation was unnecessary and is
gone now that DeadLetterEnvelope's fan-out problem (below) also
disappeared.

---

## 4. `Pipelines.Inbound.DeadLetterEnvelope` — reworked, and simplified further than Inbox (supersedes 08-30 §5)

Same check-after-error rework as Inbox, but with one further
simplification that Inbox's single-guarded-operation shape didn't need:
**`CheckingRetry`/`UpsertingRetry` are now shared across all three**
guarded operations (`Redirecting`, `Publishing`, `Producing`) — the
`For{GuardedOperation}` disambiguation from 08-29 §9d is no longer
needed, because nothing downstream needs to know *which* operation to
resume anymore. Since a check now only ever routes to `Deferring` on
"not exhausted" (never back to a specific named operation), there's
nothing left to disambiguate.

`Exit` here resolves to one of two different cross-pipeline
destinations, decided by the Router purely on which data prop is
populated — no extra query, since it's already in memory by this point:

- `DeadLetterMessage` populated (arrived via `DeadLetter`'s `Mapping`
  step) → `DeadLetterOperation.Closing`
- `Envelope` only, no `DeadLetterMessage` (arrived via the Envelope
  pipeline's `Converting` shortcut) → `EnvelopeOperation.Confirming`

This also incidentally closes a real gap in the 08-30 draft: that
version always routed `Redirecting`'s exhaustion straight to Confirming,
even on the path where a `DeadLetterMessage` row already existed — which
would have left that row open forever. Now both exhaustion and success
converge on the same `Exit`, and the Router's data-shape check handles
both correctly.

```csharp
internal enum DeadLetterEnvelopeOperation
{
    Redirecting,
    Sending,          // non-dispatchable — Router resolves to Publishing or Producing by config
    Publishing,
    Producing,
    CheckingRetry,    // shared across all three guarded operations
    UpsertingRetry,   // shared across all three guarded operations
    Deferring,
    Unrecoverable,
    Exit,             // non-dispatchable — Router resolves by which data prop is populated:
                      //   DeadLetterMessage → Closing (DeadLetterOperation)
                      //   Envelope only     → Confirming (EnvelopeOperation)
    Unknown
}
```

```csharp
internal static DeadLetterEnvelopeOperation DeadLetterEnvelopePipeline(string state) => state switch
{
    // Redirecting (side effect — no pre-check; straight attempt; any failure → shared CheckingRetry)
    RedirectDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Sending,
    RedirectDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.CheckingRetry,

    // Publishing / Producing (side effect — same shape as Redirecting)
    PublishDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Exit,
    PublishDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.CheckingRetry,
    ProducingDeadLetterEnvelopeState => DeadLetterEnvelopeOperation.Exit,
    ProduceDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.CheckingRetry,

    // CheckingRetry (shared — already known-exhausted from a prior attempt?)
    CheckRetryDeadLetterEnvelopeExhaustedState => DeadLetterEnvelopeOperation.Exit,
    CheckRetryDeadLetterEnvelopeNotExhaustedState => DeadLetterEnvelopeOperation.UpsertingRetry,
    CheckRetryDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.CheckingRetry,

    // UpsertingRetry (shared — records this attempt, decides exhaustion itself)
    UpsertRetryDeadLetterEnvelopeExhaustedState => DeadLetterEnvelopeOperation.Exit,
    UpsertRetryDeadLetterEnvelopeRetryState => DeadLetterEnvelopeOperation.Deferring,
    UpsertRetryDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.UpsertingRetry,

    _ => DeadLetterEnvelopeOperation.Unknown
};
```

Note the symmetry with Inbox's `Inserting`: no pre-check, attempt first,
`CheckingRetry`/`UpsertingRetry` only on the error path, `Deferring` for
"not yet." Same proven shape, reapplied — not a second design to
maintain mentally.

---

## 5. Major open idea, deliberately not designed yet: Router-level batch accumulation up to `Inserting`

Raised this session by Dragos, explicitly parked for after `Pipelines.Outbound.*`
and initial Router implementation — captured here so it isn't lost, not
because it's ready to build.

**The problem:** per-message processing means one DB round trip per
message just to get it durably into the Inbox table, which caps how fast
the consumer can release a partition and move on. Kafka's own consumption
model already hands the Router a natural batch (`poll()`'s return),
suggesting the fix should live in how the Router *uses* that batch, not
in the pipelines.

**The insight that simplified this considerably:** `Inserting` is the one
point in the inbound flow where *every* message's pipeline history —
regardless of path taken to get there — converges on the exact same next
operation. `ValidateInboxMessageSuccessState => Inserting` is true no
matter how a given message arrived at that state. That convergence point
is what makes batching coherent here specifically, rather than an
arbitrary place to inject accumulation logic.

**The shape settled on (conceptually, not implemented):**

- Everything from `Capturing` through `Validating` — both
  `Pipelines.Inbound.Envelope` and the `Inbox` pipeline's own
  `Validating` step — keeps running exactly as today: one message, one
  pipeline step at a time, no changes to either table.
- When the Router is about to dispatch `InboxOperation.Inserting` for a
  given message, it instead holds that message's `TData` in an
  accumulator rather than calling through immediately.
- A flush condition (size, time, or both — still open, unresolved) fires,
  the Router builds one transient batch payload from the accumulated
  `InboxMessage` props, and makes a **single** batched Insert call.
- Per-row outcomes come back from that one call, get matched to each
  original `TData`, and each message re-enters the normal single-item
  Router loop individually at whatever state its own row landed on —
  success into `Handling`, failure into `CheckingRetry` — exactly as
  already designed above, unmodified.

**Why this doesn't touch pipeline tables or operation signatures:** the
pipeline still says "next is `Inserting`" — nothing about *what* gets
picked changes, only *when* the Router chooses to act on that answer,
which has always been squarely the Router's job. The transient batch
container only needs to exist for the duration of the one Insert call —
it is not something every `TData` carries permanently, and it's not a
change to the uniform `ValueTask<(TData, string, Exception?)>` operation
signature every operation in the library shares. `Inserting` alone
becomes the operation that's collection-aware; everything upstream and
downstream of it stays exactly as today.

**Explicitly rejected alternative, for the record:** making operations
themselves collection-in/collection-out (`TData` holding
`Envelopes`/`InboxMessages`, operations looping internally and returning
a collection of states) was considered and set aside — it breaks the
uniform operation delegate shape every resolver depends on, and pushes
fan-out responsibility down into the pipeline-table layer, which has
been deliberately kept as a pure, non-executing lookup throughout this
entire design. The Router — already the one layer allowed to do more
than a lookup — is the right and only place this kind of unpacking
belongs.

**Still fully open, not to be guessed at without a dedicated session:**

- Flush trigger — size-bound, time-bound, or both.
- Exact per-row outcome extraction shape from one batched Insert
  statement (duplicate-key vs. real error vs. clean success, per row).
- Whether accumulation should also cover `Envelope`-side `Mapping` (this
  session's conversation flagged it as a "seems like it too" follow-up,
  but only `Inserting` was actually settled as the confirmed choke point
  — do not assume `Mapping` batching without re-confirming).
- Whether this pattern extends to `Pipelines.Outbound.Outbox`'s own
  Insert-equivalent once that pipeline exists — likely yes, given the
  identical shape, but unconfirmed.
- How this interacts with `IInboxData`'s generic constraints — a
  transient batch container needs its own shape, not yet sketched.

---

## 6. Still open / not yet done (carried and extended from 08-30 §6)

- **Router-level batch accumulation (§5 above)** — new, explicitly
  deferred until after `Pipelines.Outbound.*` and initial Router
  implementation exist to build against.
- **Flush-trigger design** (size/time/both) — blocks §5, not started.
- **The Publish/Produce DI-seam resolver binding mechanism** — which
  concrete delegate a broker package wires in — still undesigned, carried
  from 08-26 §10 / 08-30 §6.
- **`CircuitOpenState` variants** — whether gone everywhere as a blanket
  rule is still not explicitly confirmed, carried from 08-29 §6. Largely
  moot now for Inbox/DeadLetterEnvelope's own guarded operations (no
  self-loop left to circuit-break on), but still open for
  `Abandoning`/`Closing`/`Scheduling`'s infra-only-failure category.
- **Router-level cross-pipeline hand-off map** — Inbox's `Closing`,
  `DeadLetter`'s `Scheduling`/`Closing` entry points, and
  DeadLetterEnvelope's `Exit` → `Closing`/`Confirming` split (§4 above) —
  none formally designed yet, though the *logic* for the last one is now
  spelled out in §4 above; only the actual Router code isn't written.
- **The exact Router-generic retry-budget mechanism's API shape** — not
  coded, only the concept.
- **`Pipelines.Outbound.*`** — not started. Next planned step; the
  Publish/Produce DI seam (§6 above) applies identically to
  `Pipelines.Outbound.Envelope`, so resolving it there will likely
  resolve it here too.
- **`Persistence.RetryMessage` type's exact fields** — still only
  conceptual.
- **`OutboxMessageStatus`'s `Mapping`/`Processing` discriminator fix** —
  still parked for `Pipelines.Outbound.Outbox`.

---

## 7. How to use this doc

Load alongside all four prior handoffs — operation-level conventions
(uniform return shape, guard discipline, `PipelineError`,
non-throwing-by-contract, `IsCriticalException` routing) are unchanged
and still load-bearing. §3–4 are the current, settled shape of
`Pipelines.Inbound.Inbox` and `Pipelines.Inbound.DeadLetterEnvelope` —
supersede 08-30's versions outright. §5 is a deliberately unimplemented
idea, captured in enough detail to resume from, not a design to start
coding against yet. §6 is the active task list — most likely next step
is `Pipelines.Outbound.*`, with the Router-level batching idea (§5)
picked up once the Router itself is being built, since `Inserting` was
identified as the single natural choke point to intertwine it into. As
always: if code contradicts this doc, the code wins — re-verify before
relying on anything here.
