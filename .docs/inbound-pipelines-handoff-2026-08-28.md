# messaging-lib — Handoff (2026-08-28, Inbound Pipelines: Envelope + Inbox)

This is a **continuation** handoff, written after a session that drew the
first two concrete `Pipelines.Inbound` state→action tables and, along the
way, corrected/superseded several things in
`restructuring-pipelines-projects-handoff-2026-08-26.md`. Load this
**alongside** that doc, but treat this one as superseding wherever they
conflict — the 2026-08-26 doc's own project-structure sections (§2–3) are
now themselves partially stale (see §1 below). If code contradicts this doc,
the code wins — re-verify.

---

## 1. Golden rule — corrected wording

The 2026-08-26 doc's phrasing was imprecise. Corrected, verbatim, going
forward:

> **The Router chooses the pipeline and RUNS the actions. The Pipeline
> chooses the action.**

Two distinct responsibilities, not two decisions of the same kind:

- **Pipeline** — pure `state → action` lookup. Selects, never executes,
  never touches data beyond the state string.
- **Router** — chooses which pipeline applies (by data shape), **then
  actually calls/awaits the delegate the pipeline selected**, gets the
  resulting state back, and loops. The Router is the sole execution
  authority — this was implicit before, now explicit.

No try/catch in the Router — each operation owns its own exception handling
internally (see §4). Router-level "critical exception classification" as a
distinct concept from operation-level errors does **not** exist as a
pipeline-visible state — see §4.

---

## 2. Project structure — corrected (supersedes 2026-08-26 §2–3)

The six-project Operations split (by direction × input entity) **stays
exactly as documented in 2026-08-26**. That part is unchanged and confirmed
still correct.

**What changed: Pipelines is NOT six separate projects.** It's **two**
projects, mirroring `Routing.Inbound` / `Routing.Outbound` exactly, each
containing one **folder** per corresponding Operations project:

```
Pipelines.Inbound/
  Envelope/        ← mirrors Operations.Inbound.Envelope
  Inbox/           ← mirrors Operations.Inbound.Inbox
  DeadLetter/      ← mirrors Operations.Inbound.DeadLetter
  DeadLetterEnvelope/  ← mirrors Operations.Inbound.DeadLetterEnvelope

Pipelines.Outbound/
  Outbox/          ← mirrors Operations.Outbound.Outbox
  Envelope/        ← mirrors Operations.Outbound.Envelope
```

The six-way mirroring against Operations still holds — it just happens at
folder granularity inside two projects, not at the project level.

**Mental model (Dragos's framing, useful for future sessions):** the
inbound flow is one logical sequence cut into four segments — Envelope →
Inbox → DeadLetter → DeadLetterEnvelope — matching the operations project
order, **except** Converting jumps directly from Envelope to
DeadLetterEnvelope, skipping over Inbox/DeadLetter entirely (a genuine
shortcut, not a detour). Outbound is the same shape reversed, starting at
Outbox instead of ending there. `Envelope` is not really "shared" between
directions despite the same type/name — inbound-Envelope and
outbound-Envelope are different segments of different sequences that happen
to use the same shape.

**Type-level mirroring, per direction:** one `partial class` per direction
(e.g. `InboundFuncs`) resolves an `XxxAction` enum to the actual generic
delegate via a switch — one such resolver method per folder
(`GetEnvelopeAction`, `GetInboxAction`, etc.), all folding into the same
partial class. See §5 for the confirmed shape.

---

## 3. States: static classes with const strings — NOT enums (corrected)

2026-08-26's live-conversation idea (partial enums, one per Operations
project) is **not achievable** — C# does not support partial enums. The
workaround (static class holding const strings) is what the codebase
already had, and it's the **correct, final approach**, for a reason beyond
the language limitation: the operation delegate signature is
`ValueTask<(TData, string, Exception?)>` — uniform across every operation in
every project. An enum-per-project would break that uniformity (different
enum types couldn't share one delegate signature). `string` is required by
the delegate shape itself, not just a workaround.

So: states remain string constants, one static class per Operations
project (`EnvelopeStates`, `InboxStates`, `DeadLetterStates`,
`DeadLetterEnvelopeStates`, etc.), exactly as before. Pipelines switch on
`string state`.

---

## 4. Error classification — corrected, critical states removed

**Critical exception states no longer exist anywhere** — not in Operations,
not in Pipelines. `ConfirmEnvelopeCriticalErrorState` and all other
`...CriticalErrorState` variants have been **removed from the operations
projects**. Criticality is entirely the Router's exception-handling layer's
job (`IsCriticalException` classification), decided at the point an
exception is caught, orthogonal to and outside the pipeline's state→action
table. No pipeline state ever routes to "critical."

**New rule replacing it — pure vs. side-effect, decides self-loop vs.
`Unrecoverable`:**

- **Side-effect operations** (I/O: DB writes, broker calls, circuit
  breakers) — their `ErrorState`/`CircuitOpenState` variants **self-loop**
  back to the same action. A transient failure might genuinely succeed on
  retry.
- **Pure operations** (dev-supplied transforms marked non-throwing-by-
  contract, e.g. `Mapping`, `Converting`, and the library's own `Validating`
  functions) — any `ErrorState`/`InvalidErrorState` variant routes to a new
  action, **`Unrecoverable`**. Self-looping a pure function is pointless —
  same input, same output, every time; it would infinite-loop on a
  foregone conclusion, never self-heal.

This is a genuine reclassification, not just a rename. Notably: in
`Pipelines.Inbound.Inbox`, **both** `ValidateInboxMessageErrorState` and
`ValidateInboxMessageInvalidErrorState` now route to `Unrecoverable` —
2026-08-26 §9's claim that these route to the Mapping pipeline's Converting
step is **superseded/stale** (that whole "Mapping pipeline" sub-pipeline
concept no longer exists post-restructure per §2 above).

`EnvelopeAction`/`InboxAction` enum member is **`Unrecoverable`**, not
`Escalate` — `Escalate` was considered and explicitly rejected as a name
(kept conceptually free in case a genuine router-classified-critical
situation resurfaces at the pipeline level later; not currently used
anywhere).

---

## 5. `EnvelopeAction` resolution — confirmed shape

```csharp
partial class InboundFuncs
{
    internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
        GetEnvelopeAction<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(EnvelopeAction action)
        where TServices : IEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
        where TData : IEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>
        =>
        action switch
        {
            EnvelopeAction.Capturing  => CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
            EnvelopeAction.Validating => ValidateEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
            EnvelopeAction.Mapping    => MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
            EnvelopeAction.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
            EnvelopeAction.Confirming => ConfirmEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
            _ => default   // Unrecoverable, Exit, Unknown all fall here — Router must branch on these, none are dispatchable
        };
}
```

Confirmed intentional (not a gap): `TPayload` appears only on `MapEnvelope`'s
generic args, not on the other four — only Mapping needs it (it's the one
producing the `InboxMessage` payload shape).

Same resolver pattern applies per-folder for `Inbox`, `DeadLetter`,
`DeadLetterEnvelope` — `GetInboxAction`, etc., all on the same partial
class per direction.

---

## 6. `Pipelines.Inbound.Envelope` — settled, final

```csharp
internal enum EnvelopeAction
{
    Capturing,
    Validating,
    Mapping,
    Converting,
    Confirming,
    Unrecoverable,
    Exit,
    Unknown
}
```

```csharp
internal static EnvelopeAction EnvelopePipeline(string state) => state switch
{
    // Capturing
    NotCapturedEnvelopeState => EnvelopeAction.Capturing,        // self-loop, nothing polled
    CaptureEnvelopeErrorState => EnvelopeAction.Capturing,       // self-loop, side effect
    CaptureEnvelopeSuccessState => EnvelopeAction.Validating,

    // Validating
    ValidateEnvelopeSuccessState => EnvelopeAction.Mapping,
    ValidateEnvelopeErrorState => EnvelopeAction.Validating,     // self-loop, side effect
    ValidateEnvelopeInvalidErrorState => EnvelopeAction.Converting,       // bad message → DL path
    ValidateEnvelopeInvalidConfirmableErrorState => EnvelopeAction.Confirming,   // Envelope stays non-null; Confirmation is a property on it, not a separate branch; invalid but skip Converting — offset still confirmable

    // Mapping (pure)
    MapEnvelopeSuccessState => EnvelopeAction.Exit,              // → InboxMessage populated, Status = Mapping; router hands to Pipelines.Inbound.Inbox
    MapEnvelopeErrorState => EnvelopeAction.Unrecoverable,       // dev pure mapper, non-throwing-by-contract violated
    MapEnvelopeValueErrorState => EnvelopeAction.Converting,     // bad message → DL path

    // Converting (pure)
    ConvertEnvelopeSuccessState => EnvelopeAction.Exit,          // → DeadLetterEnvelope populated; router hands to Pipelines.Inbound.DeadLetterEnvelope
    ConvertEnvelopeErrorState => EnvelopeAction.Unrecoverable,   // dev pure FromEnvelope, contract violated
    ConvertEnvelopeInvalidState => EnvelopeAction.Confirming,    // FromEnvelope returned null → nothing to dead-letter, skip Redirecting entirely

    // Confirming
    ConfirmEnvelopeSuccessState => EnvelopeAction.Exit,          // terminal, fully processed
    ConfirmEnvelopeErrorState => EnvelopeAction.Confirming,      // self-loop, side effect

    _ => EnvelopeAction.Unknown
};
```

Notes:
- `ConfirmEnvelopeCriticalErrorState` **removed entirely** (§4).
- `EnvelopeAction.Unrecoverable`/`Exit`/`Unknown` are all non-dispatchable —
  `GetEnvelopeAction` returns `null` for all three; Router must branch on
  them explicitly, not call through.
- No `throw` in the guard clause — Router owns no try/catch, so an
  unrecognized state falls to `Unknown` rather than throwing.

---

## 7. `InboxMessageStatus` — in-memory discriminator added

Confirmed enum, still non-nullable:
```csharp
public enum InboxMessageStatus { Mapping, Processing, Handled, Abandoning, Closed }
```

**`Mapping` is new** — added this session to resolve a router-dispatch
ambiguity. Both "just built by Mapping, not yet persisted" and "persisted,
awaiting Handling" previously collapsed to the same shape
(`InboxMessage != null`, `Envelope == null`) with no discriminator, since
Mapping used to hand off `Processing` directly. Now:

- `Mapping` — assigned in-memory by the Mapping operation, never queried
  from DB, exists only briefly between Mapping and Inserting.
- `Processing` — written by `InsertInboxMessageSuccessState` as an atomic
  **transition** (not passthrough) as part of the insert write itself. This
  is what the Router keys on to route into `Pipelines.Inbound.Inbox`'s
  Handling stage.
- `Handled`, `Abandoning`, `Closed` — unchanged from 2026-08-26.

Router-level rule: `InboxMessage != null && Envelope == null`, then switch
on `Status`: `Mapping` → Validating (entry to `Pipelines.Inbound.Inbox`),
`Processing` → Handling. **This status-based entry-point dispatch is the
Router's responsibility**, not encoded inside the pipeline table itself —
the pipeline table only ever sees the operation-level state strings once
the Router has already selected which pipeline/entry to dispatch into.

**Idempotent duplicates bypass this discriminator entirely** —
`IdempotentInboxMessageState` already carries *some* prior status from the
DB (possibly past Handling, possibly `Closed`), so it's handled as its own
branch straight to `ConfirmEnvelope`, not routed by the Mapping/Processing
check.

**Parked, not yet resolved:** `OutboxMessageStatus` has the identical shape
(`Processing, Published, Abandoned`) and will hit the same collision
(Validating builds in-memory, Outbox's Transacting persists). Same fix
pattern will apply when `Pipelines.Outbound.Outbox` is drawn — explicitly
deferred, not forgotten.

---

## 8. `Pipelines.Inbound.Inbox` — settled, final

```csharp
internal enum InboxAction
{
    Validating,
    Inserting,
    Handling,
    Transacting,
    Abandoning,
    Scheduling,
    Converting,
    Closing,
    Unrecoverable,
    Exit,
    Unknown
}
```

```csharp
internal static InboxAction InboxPipeline(string state) => state switch
{
    // Validating (pure — no self-loop on either failure; supersedes 2026-08-26 §9's
    // claim that these route to a "Mapping pipeline" Converting step — that
    // sub-pipeline no longer exists post-restructure)
    ValidateInboxMessageSuccessState => InboxAction.Inserting,
    ValidateInboxMessageErrorState => InboxAction.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxAction.Unrecoverable,

    // Inserting (side effect)
    InsertInboxMessageSuccessState => InboxAction.Handling,          // atomic transition Status: Mapping → Processing
    InsertInboxMessageErrorState => InboxAction.Inserting,
    InsertInboxMessageCircuitOpenState => InboxAction.Inserting,
    IdempotentInboxMessageState => InboxAction.Exit,                 // → ConfirmEnvelope, bypassing Converting (confirmed: duplicate isn't a bad message)

    // Handling (side effect, in-memory retry budget — see §9)
    HandleInboxMessageSuccessState => InboxAction.Transacting,
    HandleInboxMessageDomainErrorState => InboxAction.Abandoning,    // domain rejection routes forward, not sideways
    HandleInboxMessageErrorState => InboxAction.Handling,            // self-loop, in-memory RetryCount already incremented, still under budget
    HandleInboxMessageExhaustedState => InboxAction.Scheduling,      // in-memory RetryCount reset to 0, hand off to persisted retry (renamed from HandleInboxMessageRetryExhaustedState — aligned to ScheduleInboxMessageExhaustedState naming)

    // Transacting (side effect)
    TransactInboxMessageSuccessState => InboxAction.Exit,            // terminal, Status = Handled
    TransactInboxMessageErrorState => InboxAction.Transacting,

    // Abandoning (side effect)
    AbandonInboxMessageSuccessState => InboxAction.Converting,
    AbandonInboxMessageErrorState => InboxAction.Abandoning,
    AbandonInboxMessageCircuitOpenState => InboxAction.Abandoning,

    // Scheduling (side effect) — entry point is Handling only (confirmed), no other entry
    ScheduleInboxMessageExhaustedState => InboxAction.Abandoning,
    ScheduleInboxMessageRetryState => InboxAction.Exit,              // retry fields persisted; next job iteration picks it up — NOT a same-run hand-off back to Handling
    ScheduleInboxMessageErrorState => InboxAction.Scheduling,

    // Converting (pure)
    ConvertInboxMessageSuccessState => InboxAction.Exit,             // → DeadLetterMessage populated, hand off to Pipelines.Inbound.DeadLetter
    ConvertInboxMessageErrorState => InboxAction.Unrecoverable,

    // Closing (side effect) — OPEN ITEM, see §10
    CloseInboxMessageSuccessState => InboxAction.Exit,               // terminal, Status = Closed
    CloseInboxMessageErrorState => InboxAction.Closing,
    CloseInboxMessageCircuitOpenState => InboxAction.Closing,

    _ => InboxAction.Unknown
};
```

---

## 9. Handling's in-memory retry budget — settled, final

Handling gets a chance at immediate in-process retry before deferring to
Scheduling's persisted/delayed retry — two independent counters, two
lifetimes:

- **In-memory** (`InboxMessage.RetryCount`, held in the object graph across
  self-loop calls within a single run, never persisted by Handling itself)
  — governs whether Handling keeps retrying immediately.
- **Persisted** (written by Scheduling) — governs delayed/next-job-run
  retry, entirely separate counter, starts fresh when Scheduling takes
  over.

**Gate checked up-front, not after a failed attempt** (Dragos's design,
preferred over an earlier draft that checked post-failure): avoids spending
an attempt (and its exception-handling cost) once the budget is already
gone. Accepted trade-off: the very last self-loop call that pushes
`RetryCount` to the threshold still returns `HandleInboxMessageErrorState`
(one more self-loop than strictly necessary); exhaustion is detected at the
**start of the next call**, which then returns `HandleInboxMessageExhaustedState`
without attempting anything. One-call lag, explicitly accepted as fine.

```csharp
internal static async ValueTask<(TData, string, Exception?)> HandleInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
    where TServices : IHandlingServices<TKey, TPayload>
    where TData : IHandlingData<TKey, TPayload>
{
    var message = RequireInboxMessage(data.InboxMessage);
    var options = services.GetInboxMessageOptions();

    if (IsMaxRetryCount(message.RetryCount ?? 0, options))
    {
        ClearInboxMessageRetryCount(message);
        return (data, HandleInboxMessageExhaustedState, null);
    }

    try
    {
        var (model, domainError) = await services.HandleInboxMessageAsync(message, ct);
        if (domainError is not null)
        {
            data.PipelineError = domainError;
            return (data, HandleInboxMessageDomainErrorState, CreateDomainException(domainError));
        }

        data.Model = model;
        return (data, HandleInboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
        data.PipelineError = exception.Message;
        IncrementInboxMessageRetryCount(data.InboxMessage);
        return (data, HandleInboxMessageErrorState, exception);
    }
}
```

- Helper renamed `VerifyMaxRetryCount` → **`IsMaxRetryCount`** (clean
  predicate, removes the awkward `!VerifyMaxRetryCount(...)` at the call
  site).
- Comparison confirmed **`>=`**, consistent with Scheduling's own
  exhaustion check — same semantics, no drift between the in-memory and
  persisted budgets.
- State renamed: `HandleInboxMessageRetryExhaustedState` →
  **`HandleInboxMessageExhaustedState`** (aligned to
  `ScheduleInboxMessageExhaustedState`'s naming; `Retry` was redundant,
  only one kind of exhaustion here).
- `HandleInboxMessageTechnicalErrorState` (2026-08-26 §9 naming) is now
  simply **`HandleInboxMessageErrorState`** — "Technical" qualifier dropped
  as redundant now that critical states don't exist and the pure/side-effect
  split (§4) does the classification work instead.

---

## 10. Still open / not yet done

- **Router-level cross-pipeline hand-off map.** Confirmed this session as a
  real, distinct, not-yet-designed piece — the Router needs some table
  mapping "this Exit from pipeline X, this state" → "enter pipeline Y at
  this point." Sharpest concrete case surfaced this session: **Closing**
  (`Operations.Inbound.Inbox`) is reached via
  `InsertDeadLetterMessageAsync → CloseInboxMessageAsync` per 2026-08-26
  §6 — but `InsertDeadLetterMessageAsync` lives in
  `Operations.Inbound.DeadLetter`, a different folder/pipeline. So the real
  sequence crosses pipelines **twice**: Abandoning → Converting → Exit to
  DeadLetter pipeline → DL's Inserting succeeds → hands back **into**
  `Pipelines.Inbound.Inbox` at Closing. Unlike every other Exit drawn so
  far (one-way), this is a two-way crossing. Not resolved — needs the
  router-level map design before Closing's routing can be considered
  final. `Pipelines.Inbound.Inbox`'s table above draws Closing as if
  self-contained/terminal, which is provisional pending this design.
- **`Pipelines.Inbound.DeadLetter`** — not yet drawn. Next planned step.
- **`Pipelines.Inbound.DeadLetterEnvelope`** — not yet drawn. Flagged as
  the sharper sequencing test (Redirecting → Publishing → Producing feels
  like an ordered sequence, not parallel branches — worth checking it's
  still a clean flat table).
- **`Pipelines.Outbound.*`** — not started at all. `OutboxMessageStatus`'s
  Mapping/Processing-equivalent discriminator fix (§7) is explicitly
  parked for when this is picked up.
- **Router wiring** — still not threaded through actual switch statements;
  everything so far is pipeline-table-level only.
- **Produce/Publish DI seam** — unchanged from 2026-08-26, still just
  directionally clarified, not designed.
- **Redelivering pipeline** — still parked, unchanged, carried forward.
- **Dynamic pipeline chaining** — still parked, confirmed Router/Routing-
  layer responsibility, unchanged from 2026-08-26.

---

## 11. How to use this doc

Load alongside `restructuring-pipelines-projects-handoff-2026-08-26.md` for
still-valid material (operation-level conventions: return shape, guard
discipline, `PipelineError`, non-throwing-by-contract, `IsCriticalException`
routing of synthetic exceptions — all unchanged) — but treat **this doc's
§1–4 as superseding** that doc's golden-rule wording, project structure,
and error-classification model wherever they conflict. §6–9 are settled,
drawn-against-actual-code state for the two pipelines completed this
session. §10 is the active task list — next step is
`Pipelines.Inbound.DeadLetter`, then `Pipelines.Inbound.DeadLetterEnvelope`,
before moving to `Pipelines.Outbound`. As always: if code contradicts this
doc, the code wins — re-verify before relying on anything here.
