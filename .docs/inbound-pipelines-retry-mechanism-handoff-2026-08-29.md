# Broker-Messaging — Handoff (2026-08-29, Retry/Exhaustion Redesign)

This is a **continuation** handoff, written after a live/voice brainstorming
session that redesigned retry and exhaustion handling for side-effect
operations across the inbound pipelines. Load this **alongside**
`inbound-pipelines-handoff-2026-08-28.md` (which itself loads alongside
`restructuring-pipelines-projects-handoff-2026-08-26.md`). Treat this doc as
superseding the 08-28 doc's §8 and §9 wherever they conflict, and as **net
new** material everywhere else. As always: if code contradicts this doc,
the code wins — re-verify before relying on anything here.

**Read this doc for the *reasoning*, not just the conclusions.** The
decisions below only make sense together — several of them look arbitrary
in isolation and only click once you see why the previous one forced it.
Skipping to a table without reading the reasoning risks reintroducing a
problem that was already ruled out this session (e.g. a fifth "retry
pipeline" — see §3 — was explicitly proposed and rejected, and the
rejection is *why* the final design looks the way it does).

---

## 1. The trigger — Handling's in-memory counter didn't generalize

08-28 §9 gave `Handling` (in `Pipelines.Inbound.Inbox`) a bespoke in-memory
`RetryCount` field, checked up-front, to bound immediate self-loop retries
before deferring to `Scheduling`'s persisted/delayed retry. This session
started by asking: do `Transacting`, `Abandoning`, `Closing`, and
`Scheduling`'s own error path need the same treatment?

The answer split the five side-effect operations in `Pipelines.Inbound.Inbox`
into two genuinely different categories — not "pure vs. side-effect" (08-28
§4's rule, which still stands for *pure* operations routing to
`Unrecoverable`), but a **new, orthogonal question specific to side-effect
operations**: *does this operation have any legitimate next action available
once its immediate retry budget is spent, or not?*

---

## 2. The Router-owned generic retry-budget mechanism (replaces all bespoke per-operation counters)

**Core decision:** retry-attempt counting and circuit-breaker behavior move
**out of individual operations entirely** and become a single generic
mechanism the Router applies uniformly to *every* side-effect operation's
plain `ErrorState`. No operation tracks its own retry count anymore.

- Every side-effect operation goes back to returning **only** a plain
  `ErrorState` on failure — no `ExhaustedState`, no in-memory counter field,
  no `IsMaxRetryCount` check inside the operation's own code.
- The Router counts attempts against the same action (keyed by
  operation/action identity). Under budget → self-loop, exactly as the
  pipeline table already says. Budget exhausted → **circuit opens** for a
  configured period; Router then resumes from exactly the same point —
  same operation, same message/data — once the window elapses.
- **08-28 §9 is retired by this change.** `HandleInboxMessageExhaustedState`,
  the in-memory `RetryCount` field, `IsMaxRetryCount`, and the up-front gate
  check inside `HandleInboxMessageAsync` are all removed. Handling's code
  reverts to the simple try/catch shape returning `HandleInboxMessageSuccessState`
  / `HandleInboxMessageDomainErrorState` / `HandleInboxMessageErrorState` only
  — the Router's generic mechanism now does what the bespoke counter used
  to do.
- **Configuration:** single shared config (attempt count, circuit-open
  duration) across all operations for now — not per-operation-tuned.
  Explicitly flagged as provisional/simplest-starting-point, not a
  considered-final decision.
- This is a genuine simplification, not a lateral move: it removes bespoke
  retry logic from Handling rather than propagating bespoke logic to four
  more operations.

**What still varies per operation is only the *consequence* of exhaustion**
— everything below this point is about that.

---

## 3. Three-way split of side-effect exhaustion consequences

Determined by asking, for each side-effect operation: **once the generic
retry budget is spent, is there any legitimate next action, or not?** This
is the operative question — explicitly **not** "DB vs. broker" (Inserting
is DB and still needs special handling; Confirming is broker and doesn't —
the axis is about available next steps, not which external system is
involved).

### 3a. Pure infrastructure-failure operations — circuit-open-and-resume, nothing else

**`Closing`, `Abandoning`, `Scheduling`'s own error path.** These are
simple status/field updates — nothing about the *message* can make these
fail; only the *mechanism* (DB down, network blip) can. There's no
per-message "poison" risk here, only transient infra failure. Exhaustion →
circuit opens, then Router resumes the exact same action on the exact same
message once the window elapses. No hand-off anywhere, no new state, no
new table involvement. This was the "simple" case Dragos identified first.

### 3b. Message-processing operations with a durable row already persisted — hand off to Scheduling

**`Handling`, `Transacting`.** Same generic self-loop/circuit-open
mechanism as 3a, but these are the two steps where a delayed retry
genuinely matters operationally (not just "resume immediately"), and
critically — by this point in the flow, `InsertInboxMessageSuccessState`
has already run, so a persisted `InboxMessage` row exists. Exhaustion
routes to **Scheduling**, which persists `RetryCount`/`NextAttemptAt` onto
that existing row (an `UPDATE`) — this is mechanically only possible
*because* the row already exists.

**How the Router decides "this one hands off to Scheduling, that one just
reopens":** a small **Router-owned mapper table** — structurally identical
in kind to `GetEnvelopeAction`/`InboxPipeline` themselves (an
`(operation/action) → destination` lookup), just living one layer up, at
the Router's retry-orchestration layer rather than pipeline-selection.
**The operation itself never knows or returns anything indicating it
should route to Scheduling** — it always returns the same plain
`ErrorState` regardless of attempt count; the mapper table is what the
Router consults *after* its own generic exhaustion detection fires, to
decide where (if anywhere) to redirect. This preserves the rule that
operations are dumb leaf actions and all sequencing intelligence lives at
the pipeline-table/Router level, not inside operation bodies.

### 3c. Pre-durable operations with no row to schedule against — new `RetryMessage` mechanism

**`Inserting`** (Inbox), and — parked for when those pipelines are drawn —
**`Publishing`, `Producing`, `Redirecting`** (DeadLetterEnvelope). These
fail *before* any row exists that Scheduling could `UPDATE`. Inserting's
whole job is creating the row that would be needed; if Inserting itself is
exhausted, there is nothing to schedule against. This is also explicitly
identified as **the first point where a genuine "poison message" can
occur** in the sense of "will deterministically fail forever," not just
"transient outage" — same failure shape a pure function's deterministic
error has, but arising in a side-effect operation.

Full design in §4.

### 3d. No-alternative operations — infinite circuit-open, by design, no table at all

**`Capturing`, `Confirming`** (Envelope pipeline). Explicitly and
deliberately **excluded** from the §3c retry-table treatment, for a
mechanical reason, not just "these are extra-important":

- **Confirming is what advances the broker offset.** If an operation that
  fails *before* Confirming (Inserting, Publishing, etc.) retried forever
  with no exhaustion path, the envelope could never reach Confirming, the
  offset would never advance, and the consumer would be permanently stuck
  on one envelope, blocking everything behind it. That's *why* §3c
  operations need a bounded budget followed by an escape to Confirming.
- **Capturing and Confirming themselves have no such escape**, because
  there is nothing conceptually earlier or later to hand off to — if the
  confirmation mechanism itself (or the broker connection Capturing needs)
  is down, that's not a per-message problem, it's the consumer mechanism
  itself being non-functional. Skipping ahead is not possible or
  meaningful; blocking is the only honest behavior.
- Consequence: self-loop under the generic budget, then **circuit-open
  forever**, periodically retrying the same action indefinitely. No retry
  table, no hand-off, no new state. If this never recovers, it's a
  human-intervention case (visible via circuit-breaker logs/metrics) — the
  library has no further move available, by design, not by oversight.

---

## 4. The `RetryMessage` mechanism (§3c) — full design

### 4a. Purpose and what it is *not*

A durable record of "this specific envelope has failed to insert/publish/
produce/redirect N times, is it exhausted yet." It plays the same
structural role for **pre-insert** envelopes that `InboxMessage.RetryCount`/
`NextAttemptAt` plays for **post-insert** messages (§3b) — but anchored to
envelope identity, since that's the only identity available at this stage.

**Explicitly not a message store.** The table holds identity plus retry
bookkeeping fields only (mirroring `DeadLetterMessage`'s existing retry
fields — count, next-attempt, etc.) — it **never stores the envelope
payload itself**. On every retry attempt, the envelope is re-obtained fresh
from the broker (which is already the durable store for it), not
deserialized from this table. Rationale (Dragos, explicit): avoids
designing/versioning a serialization format, avoids duplicate storage,
avoids staleness risk between what's stored and what the broker actually
holds.

**Secondary, independently valuable benefit** (raised near the end of the
session, worth preserving as a design rationale, not just a side effect):
this table is a genuine **queryable, durable audit trail of poison
envelopes** — not just something developers have to grep logs for. That's
real product value on its own, separate from the retry mechanism itself.

### 4b. Identity / key

Composite key: **envelope key + envelope date**. Chosen specifically
because this is the *only* identity that exists before an `InboxMessage`
row is created — Mapping has run (producing an in-memory `InboxMessage`),
but Inserting hasn't succeeded, so there's no `InboxMessage` identity yet
to key against.

### 4c. Where Checking and Upserting live — SUPERSEDED 2026-08-30 evening, see §9

> **Correction below in §9.** As first written, this subsection proposed
> a new dedicated `Operations.Inbound.RetryMessage` project for Checking
> and Upserting. That placement didn't survive implementation either —
> Dragos concluded it was an artificial home invented just to hold two
> functions, not a place they naturally belonged. §9 moves them one layer
> down, into `Persistence.RetryMessage` itself. Treat §9 as authoritative
> for where these two functions live; the paragraph below is preserved
> for the reasoning about *why they're their own thing at all*, which
> still holds.

Explicit decision (Dragos, direct quote of intent): treat this with **full
architectural consistency**, exactly like every other entity in the
system — new `Persistence.RetryMessage`-shaped type at Layer 1, its own
Operations project at Layer 2 (naming TBD — something like
`Operations.Inbound.RetryMessage`), same conventions as `DeadLetter`/
`Outbox`/etc. Rejected alternative: implementing this as ad-hoc logic
living inside the Router's generic mechanism from §2 — deemed inconsistent
with how every other piece of state in this system is treated.

### 4d. The two operations: Checking and Upserting — REVISED 2026-08-30, see §9

> **This subsection was rewritten the morning after the session that
> produced it, once Dragos actually sat down to implement it.** The
> original version below (§4d as first written) assumed Checking and
> Upserting each took a full `IEnvelopeData`/`IDeadLetterEnvelopeData` and
> returned pipeline states directly. That assumption didn't survive
> implementation — see §9 for the full story and reasoning. **What
> follows is the corrected, current design; treat it as authoritative.**

Two operations, not more:

- **Checking** — a **pre-check gate**, run **before**
  Inserting/Publishing/Producing/Redirecting are even attempted, not just
  a consequence of them failing. Signature takes **only the retry
  identity** — envelope key plus envelope date, as plain parameters, not
  an `IEnvelopeData`/`IDeadLetterEnvelopeData` object. Returns a plain
  **`bool`** — is this envelope already marked exhausted, yes or no. It
  does **not** return a pipeline state; see §4f for why.
- **Upserting** — insert-if-absent-else-increment, one atomic operation.
  Same reduced signature: just envelope key plus envelope date. Returns
  **`Task`** — nothing, no value, no state. Called only when the guarded
  operation (Inserting, etc.) itself exhausts *its own* generic retry
  budget (§2).

Neither operation performs `Require*` guard checks or wraps its own body
in try/catch for the caller's data shape — see §4f, that responsibility
moved to the wrappers along with the type-specific extraction.

**Critical distinction that makes Upserting safe to retry indefinitely:**
unlike Inserting/Handling/Transacting (which touch broker- or
domain-supplied data that can be genuinely poisoned), the retry table's own
row content is **entirely under the library's own control** — it's not
shaped by whatever the broker handed it. So Upserting itself **cannot** be
poisoned in the same sense. Therefore: **Upserting self-loops
indefinitely** on its own failure (a genuine exception still propagates
out of it — its *caller*, the wrapper, is what decides what to do with
that, per §4f), no exhaustion, no escalation — same philosophy as §3d's
infinite circuit, but applied here because the operation is provably
always-eventually-succeeds-safe, not because there's nowhere else to go.

### 4e. Where Checking and Upserting physically live — REVISED 2026-08-30 morning, SUPERSEDED again 2026-08-30 evening, see §9

Checking and Upserting live in their own small Operations project (e.g.
`Operations.Inbound.RetryMessage`), exactly like the
`Persistence.RetryMessage` type itself is its own thing (§4c) — **this
part is unchanged**. They are genuinely shared, single implementations,
called from four separate places.

**What changed:** the four call sites are no longer envisioned as rows
directly referencing Checking/Upserting's own delegate. Instead, each call
site is a **wrapper** (§4f) — and the wrapper's location was revisited
too. A dedicated shared project for the wrappers themselves was
considered and **explicitly rejected** by Dragos: he does not want a
separate shared "inbound pipelines wrappers" project. Instead, **each
wrapper lives directly inside the pipeline folder that guards it** —
the wrapper for Inserting lives inside the `Inbox` folder in
`Pipelines.Inbound`; the three wrappers for Publishing, Producing,
Redirecting live inside the `DeadLetterEnvelope` folder in
`Pipelines.Inbound`. No new folder, no new project, nothing shared at the
Pipelines layer at all — the *only* shared artifact in this whole
mechanism is Checking/Upserting themselves, in their one small Operations
project. This keeps every pipeline folder self-contained and locally
ownable, consistent with how every other pipeline folder in the codebase
already looks.

(Historical note, still valid reasoning: two other structural options —
calling Checking/Upserting *from inside* Inserting's own function body,
and a dedicated fifth "retry pipeline" — were considered and rejected
earlier in the original session for the reasons described in the original
§4e text below. Those rejections still hold; only the *wrapper's* location
changed on 08-30, not whether a wrapper-shaped thing is needed at all.)

### 4f. What the wrappers actually do — REVISED 2026-08-30 morning, SUPERSEDED again 2026-08-30 evening, see §9

The wrappers are where all the type-specific and pipeline-specific work
now concentrates — this is the single biggest shift from the original
same-day design:

- **Type-specific extraction and guarding.** Each wrapper knows its own
  concrete data type (`IEnvelopeData` for the Inbox wrapper;
  `IDeadLetterEnvelopeData` for each of the three DeadLetterEnvelope
  wrappers). It performs the `Require*` guard (e.g. `RequireEnvelope`,
  `RequireDeadLetterEnvelope`) and pulls out envelope key and date from
  whatever shape it was handed.
- **Try/catch.** The wrapper — not Checking/Upserting themselves — owns
  the try/catch around the call. This is a direct consequence of the
  above: since the wrapper is now the first place the type-specific data
  is actually touched, it's also the natural place for the operation's
  usual exception-to-state translation to happen, same as every other
  operation in the codebase.
- **State translation.** Checking returns a bare `bool`; Upserting returns
  a bare `Task`. The wrapper is what turns "exhausted: true/false" or
  "upsert completed / threw" into the pipeline's own **distinctly-named
  states** — e.g. (naming still TBD, pattern confirmed)
  `CheckRetryMessageForInsertingExhaustedState` /
  `...NotExhaustedState` for the Inbox wrapper, and three analogous pairs
  for the DeadLetterEnvelope wrappers (`...ForPublishing...`,
  `...ForProducing...`, `...ForRedirecting...`).

**Why this settles the original 08-29 state-naming/fan-out problem
differently than first written:** the original §4f (see below) assumed
Checking/Upserting themselves would need to return distinctly-named
states, which was awkward given they're supposed to be genuinely shared.
The 08-30 correction removes the tension entirely — the shared operations
never speak in states at all, only in bool/Task, so there's nothing to
disambiguate at that layer. Disambiguation now happens exactly once, in
each wrapper, which is exactly where it belongs per the golden rule: an
Operations-layer thing (Checking/Upserting) cannot know about
pipeline-layer concepts (states) in the first place, so this correction
isn't just a naming convenience — it fixes a layering violation the
original design had smuggled in.

**Original 08-29 §4e/§4f reasoning, preserved for context (superseded by
above, but the *rejections* below still stand):**

> Both `Inserting` (lives in `Pipelines.Inbound.Inbox`) and
> `Publishing`/`Producing`/`Redirecting` (live in
> `Pipelines.Inbound.DeadLetterEnvelope`) need the identical Checking/
> Upserting logic. Two structural options were considered and rejected
> before landing on the final shape:
>
> - **Rejected: call Checking/Upserting *from inside* Inserting's own
>   function body.** This breaks the fundamental discipline that has held
>   everywhere else — operations are leaf actions, pipelines (via their
>   state→action tables) do all sequencing; no operation should
>   orchestrate another operation internally.
> - **Rejected: a dedicated fifth "retry pipeline."** Reintroduces the
>   cross-pipeline jump problem and doesn't fit the four-segment mental
>   model (08-28 §2) — a Checking/Upserting pair isn't a segment of the
>   inbound sequence, it's a cross-cutting concern consulted *from*
>   segments.
>
> The originally-proposed final shape — Checking/Upserting's own
> delegates referenced directly as pipeline-table rows, each returning
> its own generic state — is what §4d/§4e/§4f above correct. The
> rejections of the two bullets above are unaffected by that correction;
> only the "how do the four call sites talk to the shared operations"
> answer changed, from "direct table reference with generic states" to
> "wrapper owns the type + the state translation."

### 4g. Full sequence, stated end to end

For **Inserting** specifically (the only one of the four fully drawn so
far — Publishing/Producing/Redirecting follow the identical shape once
`Pipelines.Inbound.DeadLetterEnvelope` is drawn):

1. Pipeline enters at the Inbox folder's Checking wrapper (before
   Inserting is ever attempted).
2. The wrapper guards/extracts key+date from its `IEnvelopeData`, calls
   the shared Checking operation, and translates its `bool` into one of
   this wrapper's own two states.
   - **Exhausted (`true`)** → skip Inserting entirely, route straight to
     **Confirming** (§4h — this is a cross-pipeline `Exit`, same kind as
     Mapping/Converting's existing exits, not a new mechanism).
   - **Not exhausted (`false`)** → proceed to Inserting normally.
3. Inserting runs, self-loops on its own plain `ErrorState` under the
   Router's generic budget (§2), exactly as before.
4. If Inserting's generic budget is exhausted (§2's circuit-open logic
   fires) → **this is the one case in §3 that does NOT just reopen the
   circuit** — instead, the Inbox folder's Upserting wrapper is invoked,
   which guards/extracts key+date and calls the shared Upserting
   operation to write/increment the retry record for this envelope.
5. Upserting retries indefinitely until it succeeds (§4d — cannot be
   poisoned, safe to loop forever; a genuine exception from it is caught
   by the wrapper, per §4f, and translated into that wrapper's own
   error state, which presumably self-loops back to retrying Upserting —
   exact shape TBD).
6. Once the Upsert succeeds, route to **Confirming** — this run gives up on
   Inserting for now, but the offset still advances; the *next* time this
   same envelope is encountered (if it ever is again, which for Inserting
   specifically may not recur the same way it does for Handling/
   Transacting — TBD/not fully specified this session), Checking's
   pre-check gate is what prevents re-attempting a known-exhausted
   envelope.

### 4h. Why exhaustion routes to Confirming, not Converting

Converting (Envelope pipeline) needs a populated `InboxMessage` to build a
`DeadLetterMessage` from. Inserting failing means that object was built by
Mapping but never persisted — there's no successfully-inserted state to
convert *from* in the usual sense, and more importantly, **the offset must
still advance** or the consumer is permanently stuck (see §3d's reasoning
— this is the same underlying constraint, just viewed from the opposite
side). So exhaustion routes directly to Confirming, bypassing Converting
entirely. The retry table itself (§4a) is what serves as the forensic
record of what got skipped, in place of a proper dead-lettered message.

---

## 5. Outbound pipeline — brief note, not fully designed this session

Flagged explicitly as **structurally simpler** than the inbound retry
mechanism, and *why*: the outbound pipeline has a **synchronous caller**
sitting right there (the developer's own code invoking the library), not
just an asynchronous job/consumer loop. So failures in outbound operations
(post-Validating, e.g. Transacting the model + outbox message) can just
**fail back to the developer's call immediately** — no retry table, no
Router-generic budget-then-escalate mechanism needed for that path. A job
will still exist to rerun from persisted retry points for already-enqueued
messages, but the "developer finds out immediately" path means outbound
doesn't need the same apparatus inbound does. **Not designed further this
session** — noted so it isn't lost, and so `Pipelines.Outbound.*` isn't
approached by reflexively copying the inbound retry design without
reconsidering whether it's needed.

---

## 6. What changed vs. 08-28 doc, concretely

- **08-28 §8** (`InboxPipeline` table): `HandleInboxMessageExhaustedState`
  routing to `Scheduling` is **still directionally correct** (§3b above),
  but the *mechanism* by which exhaustion is detected changes — it's no
  longer Handling's own in-memory counter (see next point), it's the
  Router's generic mechanism (§2) plus the mapper table (§3b). The pipeline
  table's row `HandleInboxMessageExhaustedState => InboxAction.Scheduling`
  itself doesn't need to change, but how that state gets produced does.
  Actually — **reconsider whether `HandleInboxMessageExhaustedState` is
  still a state the operation returns at all**, versus the Router
  intercepting before ever calling Handling again once its budget is
  spent, and redirecting without Handling ever seeing/returning an
  "exhausted" state. This distinction was raised as an open question by
  Claude mid-session and **not explicitly resolved by Dragos** — flagged
  as needing a decision, not settled.
- **08-28 §9** (Handling's in-memory retry budget, full code block):
  **retired**. `IsMaxRetryCount`, `ClearInboxMessageRetryCount`,
  `IncrementInboxMessageRetryCount`, and the up-front gate check are all
  removed from `HandleInboxMessageAsync`. Handling reverts to a simple
  try/catch returning only Success/DomainError/Error states. Replaced by
  §2's Router-generic mechanism.
- `InsertInboxMessageErrorState`/`InsertInboxMessageCircuitOpenState`
  self-looping to `Inserting` (08-28 §8) is now understood as **only part
  of the picture** — needs the new pre-check (Checking) and post-exhaustion
  (Upserting, then Confirming) steps added around it per §4g.
- **`CircuitOpenState` variants** (e.g.
  `InsertInboxMessageCircuitOpenState`,
  `AbandonInboxMessageCircuitOpenState`,
  `CloseInboxMessageCircuitOpenState` in 08-28 §8's table): given §2 moves
  circuit-breaker behavior to a Router-generic mechanism rather than
  per-operation state, **these explicit state variants may no longer be
  needed** — the Router's generic mechanism may not need the operation to
  ever return a distinct `CircuitOpenState` at all, since the Router itself
  now owns the concept of "circuit is open" rather than being told about it
  via a returned state. **Not explicitly confirmed by Dragos this
  session** — flagged as a likely simplification to verify.

---

## 7. Still open / not yet done (supersedes/extends 08-28 §10)

- **The exact mechanism/API shape for the Router-generic retry-budget
  mechanism (§2)** — not coded this session, only the concept and the
  mapper-table idea for §3b. Needs an actual design pass: how is the
  mapper table structured, where does attempt-count state live between
  Router invocations, how does "resume from the same point" actually work
  mechanically.
- **Whether `HandleInboxMessageExhaustedState`-style states still exist as
  operation-returned states at all**, or whether the Router intercepts
  before ever re-invoking the operation — explicitly raised, explicitly
  unresolved (§6).
- **Whether `CircuitOpenState` operation-level states are still needed**
  given §2 — flagged as likely-obsolete, not confirmed (§6).
- **Exact final naming** for: the new `RetryMessage`-shaped Persistence
  type, its Operations project, the Checking and Upserting operations
  themselves, and all the per-pipeline wrapper states in §4f — all
  explicitly TBD. The *shape* is now more settled than before (§9):
  Checking/Upserting take key+date and return bool/Task; four wrappers,
  one per guarded operation, live in their own pipeline folders and own
  the states — but concrete names still need to be chosen.
- **What (if anything) re-attempts an exhausted-and-Confirmed envelope
  later** — §4g step 6 flags this as unspecified. Confirmed: no automatic
  un-exhaust mechanism, no human-workflow designed — Dragos explicitly said
  this is intentionally left with no un-exhaust path, pure forensic record,
  human-only intervention, matching §3d's philosophy. Not a gap, a
  deliberate choice — but *how* a human would act on it (query the table?
  a dashboard? nothing built yet) is unaddressed.
- **`Publishing`/`Producing`/`Redirecting`'s own Checking/Upserting
  wrapper states** (§4f) — pattern confirmed, not concretely drawn, since
  `Pipelines.Inbound.DeadLetterEnvelope` itself isn't drawn yet.
- **Router-level cross-pipeline hand-off map** (carried from 08-28 §10,
  unchanged) — still not designed. Now has **two** concrete cases needing
  it instead of one: the original Closing two-way crossing, plus §4g's
  Checking-exhausted → Confirming crossing (though the latter is one-way,
  same kind as Mapping/Converting's existing exits, so likely simpler).
- **`Pipelines.Inbound.DeadLetter`** — not yet drawn. Still next planned
  step per 08-28.
- **`Pipelines.Inbound.DeadLetterEnvelope`** — not yet drawn. Now known to
  need: the ordered Publishing → Producing → Redirecting sequencing
  (08-28's original flag) **plus** three Checking/Upserting wrapper
  call-sites per §4f.
- **`Pipelines.Outbound.*`** — not started. §5's note (synchronous-caller
  failure path, likely doesn't need the retry-table apparatus) should be
  revisited and either confirmed or overturned when this is picked up, not
  assumed. `OutboxMessageStatus` discriminator fix (08-28 §7) still
  separately parked.
- **`Persistence.RetryMessage` type's exact fields** — only "mirrors
  `DeadLetterMessage`'s retry fields" specified; not drawn concretely.

---

## 8. Follow-up (2026-08-30) — Envelope vs. DeadLetterEnvelope gap found during implementation, and its resolution

While actually implementing §4, Dragos hit a real design gap: Checking and
Upserting as originally written implicitly assumed one shared data shape,
but `Inserting`'s guard needs `IEnvelopeData` while
`Publishing`/`Producing`/`Redirecting`'s guards need
`IDeadLetterEnvelopeData` — genuinely different types, even though their
relevant fields overlap. A single non-generic implementation can't take
both.

First instinct (evening of 08-29): make it four operations instead of
two — one Checking/Upserting pair per data type, same logic, separate
implementations. Workable, but doubled the surface area for identical
logic.

Better idea, arrived at the next morning once Dragos had slept on it:
**Checking and Upserting never needed the full envelope data in the first
place** — the retry table is keyed on envelope key + envelope date only
(§4b), so that's all the operations actually need as input. Reduce their
signatures to just those two primitives, and they're type-agnostic by
construction — back to two operations, not four, with no generic-type
machinery needed either.

That reopened a question about where the type-specific work — the
`Require*` guard, extracting key/date out of whichever concrete data
type a call site has — should live, given Operations-layer code can't
reference another Operations project (layering invariant: each layer may
only reference the layer immediately below it — Operations may reference
Persistence/Transport, but not another Operations project; Pipelines may
reference Operations; Router may reference Pipelines). Resolution,
reached across the conversation:

- Checking returns a plain `bool` (exhausted, yes/no); Upserting returns
  a plain `Task` (no value). **Neither returns a pipeline state** — state
  is a Pipelines-layer concept, and Checking/Upserting have no pipeline
  table of their own, so they were never the right place for state to
  originate.
- The `Require*` guard and the try/catch both move to the **wrapper** at
  each of the four call sites — the wrapper is the first code that
  actually touches the concrete `IEnvelopeData`/`IDeadLetterEnvelopeData`
  shape, so it's the natural (and, per the layering invariant, the only
  legal) place for that type-specific work and for translating a caught
  exception or a `bool` into this call site's own named states.
- The four wrappers do **not** get a new shared project. Dragos explicitly
  didn't want a dedicated "wrappers" project. Each wrapper lives directly
  inside the pipeline folder it guards — Inserting's wrapper inside the
  `Inbox` folder, the other three inside the `DeadLetterEnvelope` folder —
  keeping every pipeline folder self-contained, matching how the rest of
  the codebase is already organized.

Net effect: Checking and Upserting (§4d) are now smaller and simpler than
originally drafted — pure functions of key+date, no guards, no states,
no try/catch of their own. All the type-awareness and pipeline-awareness
that was originally (incorrectly) pushed onto them now sits correctly at
the four wrapper call sites, which is also where the original
state-fan-out problem (old §4f) resolves for free, since each wrapper only
ever needs to name states for the one operation it's guarding. §4d, §4e,
§4f above have been rewritten in place to reflect this; the original
reasoning is preserved inline (in §4f) since the rejections it contains
are still valid, only the final shape changed.

Also worth preserving as context, not as part of the design: Dragos spent
close to ten hours across roughly a dozen implementation attempts the
prior evening trying to get this same design implemented with another
tool/model combination, without success, despite the design itself being
fully worked out and despite the codebase already containing extensive,
consistent precedent for every pattern involved. He described the
experience as consistent with complaints he's heard from other
developers — that AI models can be very strong at the brainstorming and
architectural-reasoning phase, but comparatively weak at faithfully
applying an established pattern once the reasoning is done. Flagged here
only because it's the reason this handoff exists in this much detail —
the intent is for the *next* implementation attempt to have enough
concrete, unambiguous context to succeed on the mechanical part, not just
the design part.

---

## 9. Follow-up (2026-08-30, evening) — final layering correction, plus naming patterns

Dragos implemented the 08-30-morning design (§8) and, in doing so, found
one more placement problem — this time not about types, but about layers.

### 9a. Checking/Upserting move down to Persistence; wrappers move up to Operations

The dedicated `Operations.Inbound.RetryMessage` project proposed for
Checking and Upserting (original §4c/§4e) was, on reflection, **artificial
— invented just to give these two functions a home**, not a place they
actually belonged by the same logic used everywhere else in the codebase.
Once Checking and Upserting were reduced to taking only envelope key +
date and returning bare `bool`/`Task` (§8), they no longer touch
`IEnvelopeData`, `IDeadLetterEnvelopeData`, or anything Operations-layer
at all — they're purely "get/update a row in the retry table by its own
key," which is exactly what a **Persistence-layer** service does for any
other entity in the system. So:

- **Checking and Upserting move down one layer, into
  `Persistence.RetryMessage` itself**, right next to the
  `RetryMessage`-shaped type and its own DB access, same as how Inbox's
  or Outbox's persistence services work.
- **The wrappers move up, out of the Pipelines folders, into
  `Operations.Inbox` and `Operations.DeadLetterEnvelope`.** Symmetric
  reasoning: putting them in Pipelines was equally artificial once they
  became the thing doing the `Require*` guard, the try/catch, and the
  type-specific extraction (§8/§4f) — that's exactly what an ordinary
  Operations-layer function looks like everywhere else in this codebase,
  so Operations is where they were always going to end up structurally.

This is a genuine fix, not just a reshuffle: it removes the one place in
the whole design that had needed an invented, one-off project just to
exist, and both ends of the move land on layers that already have an
established, consistent role for exactly this kind of code.

### 9b. Wrappers now behave like every other operation — full count, CORRECTED after seeing actual code

Because the wrappers now live in Operations, they fall back in line with
how every other function in those projects already behaves: their own
`Require*` guards, their own try/catch, and — unlike the 08-30-morning
version — they **return actual pipeline-facing states directly**, not a
bare `bool`/`Task` translated by something else. There is no longer a
translation step separate from the operation itself; the wrapper *is* the
operation, in the same sense `InsertInboxMessageAsync` or any other
Operations-layer function already is. Every wrapper returns the standard
three-way shape `(TData, string, Exception?)` and has **three** states —
`...ExhaustedState` / `...NotExhaustedState` for Checking, or
`...SuccessState` / `...ErrorState` for Upserting, plus an `...ErrorState`
for Checking too (see below) — matching the convention every other
operation in the codebase already follows for its own error path.

**Total count, seen against actual implemented code: six wrapper
operations, not eight** — this corrects §9b as first drafted, which
assumed Upserting needed one instance per guarded operation the same way
Checking does. It doesn't:

- **Checking — four wrappers, one per guarded operation** (fan-out
  reasoning in §9d still holds): `CheckRetryInboxMessageForInsertingAsync`
  in `Operations.Inbox`, and
  `CheckRetryDeadLetterEnvelopeForPublishingAsync` /
  `...ForProducingAsync` / `...ForRedirectingAsync` in
  `Operations.DeadLetterEnvelope`.
- **Upserting — two wrappers, one per *pipeline*, not per guarded
  operation**: `UpsertRetryInboxMessageAsync` in `Operations.Inbox`
  (called only after `Inserting` exhausts, since that's Inbox's only §3c
  operation), and `UpsertRetryDeadLetterEnvelopeAsync` in
  `Operations.DeadLetterEnvelope` (called after *any* of Publishing/
  Producing/Redirecting exhausts — all three converge on the same single
  call, since **once past exhaustion, which operation triggered it no
  longer matters** — see §9d for why this is the right cut, not a
  shortcut).

`Persistence.RetryMessage` itself holds exactly two functions, shared by
all six wrappers above: **`CheckRetryMessageExhaustedAsync`** (not
`CheckRetryMessageAsync` as originally drafted — the actual verb reflects
what it returns, a bool answering "is this exhausted") and
**`UpsertRetryMessageAsync`**, whose signature grew one parameter beyond
what §4d originally specified — see §9e.

### 9c. Naming pattern — CORRECTED after seeing actual code

- **Persistence-layer, shared, type-agnostic:**
  `CheckRetryMessageExhaustedAsync`, `UpsertRetryMessageAsync`. Generic on
  purpose — they take only key+date(+error message, for Upsert — §9e),
  know nothing about Envelope/DeadLetterEnvelope/InboxMessage, so the
  generic word "Message" is correct here, not a placeholder.
- **Operations-layer wrappers, general pattern:**
  `{Verb}Retry{Entity}[For{GuardedOperation}]Async` — verb first, then
  "Retry", then the entity name, then (for Checking only, per-guarded-
  operation — §9d) a `For{GuardedOperation}` suffix using that operation's
  own gerund name. Concretely, as implemented:
  - `CheckRetryInboxMessageForInsertingAsync` (Inbox)
  - `CheckRetryDeadLetterEnvelopeForPublishingAsync` /
    `...ForProducingAsync` / `...ForRedirectingAsync`
    (DeadLetterEnvelope)
  - `UpsertRetryInboxMessageAsync` (Inbox — no `For...` suffix, §9b)
  - `UpsertRetryDeadLetterEnvelopeAsync` (DeadLetterEnvelope — likewise
    no `For...` suffix)
- **The Inbox Checking wrapper's entity name is `InboxMessage`, not
  `Envelope`** — a real, deliberate choice, not just naming: its retry
  identity is sourced from `RequireInboxMessage(data.InboxMessage)`'s own
  `MessageKey`/`CreatedAt`, i.e. from the in-memory `InboxMessage` Mapping
  already built, not from the raw broker envelope directly. Both exist on
  `data` at this point in the pipeline; using the `InboxMessage`'s own
  identity was the deliberate choice. (§4b's original "envelope key +
  envelope date" wording still describes the *concept* correctly —
  they're the same underlying key/date, just read off the `InboxMessage`
  object rather than the `Envelope` object for this particular call
  site.)
- Consistency was explicitly chosen over brevity — the names are long
  (`CheckRetryDeadLetterEnvelopeForPublishingNotExhaustedState` is the
  longest single example raised), and Dragos confirmed that's an accepted
  tradeoff, not a problem to solve around.

### 9d. Why Checking's *states* need per-guarded-operation disambiguation, but Upserting's don't (extends to the wrapper *count* too, not just naming — CORRECTED)

This took real back-and-forth to land on, so the reasoning is worth
keeping, not just the conclusion. The 09-04-morning-vs-evening exchange
also clarified this cuts deeper than state naming — it determines the
*wrapper count* too (§9b), not just what each wrapper's states are called:

- **Checking has a live fan-out problem that only per-call-site
  wrappers/states can solve.** Before this mechanism existed, each of
  Inserting, Publishing, Producing, and Redirecting was reached directly
  from whatever state preceded it in its pipeline table. Now Checking
  sits in front of each, and when it reports "not exhausted," the table
  needs to know **which one of the (up to three, for DeadLetterEnvelope)
  operations to resume** — that information has nowhere else to live
  except in the state string itself, since the router dispatches purely
  on state and nothing else survives the round-trip. So Checking needs
  **one wrapper instance per guarded operation** (four total, §9b), each
  with its own three states:
  `CheckRetry{Entity}For{Operation}ExhaustedState`,
  `...NotExhaustedState`, and `...ErrorState` for the DB-lookup's own
  failure path (this third state was missing from the doc as first
  drafted — every wrapper needs an error path the same as any other
  operation; it wasn't a new decision, just an omission caught once real
  code was written).
- **Upserting has no such fan-out to encode, and this turned out to mean
  no separate instances either, not just shared state names.** Once past
  exhaustion, *which* operation triggered it no longer matters — Inbox's
  Upserting always hands off to the same place (Scheduling/Confirming per
  §3b/§4h reasoning) regardless of the fact that only Inserting could ever
  call it; DeadLetterEnvelope's Upserting likewise always converges on
  that pipeline's own single end-of-pipeline destination regardless of
  whether Publishing, Producing, or Redirecting was the one that
  exhausted. Since the *destination* doesn't vary, there's no reason to
  duplicate the *wrapper*, not just the states — hence two Upserting
  wrappers total (one per pipeline), not four or six. Each has two
  states, `...SuccessState`/`...ErrorState` — no `Exhausted`/`NotExhausted`
  pair, since Upserting isn't answering a yes/no question, it's just
  performing a write that either succeeds or throws.

### 9e. `UpsertRetryMessageAsync`'s signature grew one parameter — CORRECTED, was missing from §4d

As actually implemented, the Upserting wrappers build an error message —
`data.PipelineError ?? "Unknown upsert retry {entity} error"` — and pass
it through to `Persistence.RetryMessage`'s `UpsertRetryMessageAsync`,
whose real signature is
`UpsertRetryMessageAsync(services, key, date, error, ct)`, not just
`(services, key, date, ct)` as §4d originally specified. This wasn't
caught during design — it surfaced once real code was written — but it's
a good addition, not scope creep: without it, the retry table would only
ever hold a count and a timestamp, and §4a's claim that this table
doubles as **"a genuine, queryable audit trail of poison envelopes"**
would be hollow — there'd be nothing forensic to actually look at. With
the error message persisted alongside the count, that claim is now
actually true of the implementation, not just aspirational.


---

## 10. How to use this doc
`restructuring-pipelines-projects-handoff-2026-08-26.md`. This doc's §1–6
are net-new material from a live/voice session — read the reasoning, not
just the tables, since several conclusions (especially §4e's rejection of
a fifth pipeline, and §4f's wrapper-state resolution) only make sense in
light of alternatives that were explicitly proposed and ruled out. §7 is
the active task list. Next concrete step, per Dragos: before writing more
code, the open items in §7 (especially the Router-generic mechanism's
actual shape, and whether operation-level Exhausted/CircuitOpen states
still exist) likely need a further session to resolve — this doc captures
where reasoning currently stands, not a finished spec ready to implement
blind. If code contradicts this doc, the code wins — re-verify.
