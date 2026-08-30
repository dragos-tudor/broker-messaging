# Kafka Library — Handoff (2026-08-26, Router/Pipeline Architecture session)

This is a **continuation** handoff, written after a session that redesigned the
project structure and routing model from the ground up. Load this **alongside**
the three 2026-08-25 docs (`kafka-lib-handoff-2026-08-25.md`,
`deadletter-operations-handoff-2026-08-25.md`,
`outbox-operations-handoff-2026-08-25.md`) — but be aware this session
**supersedes their project-structure sections** (they describe a 5-package /
Operations-by-message-type model that this session replaced). Operation-level
conventions (return shape, guard discipline, `PipelineError`, synthetic
exceptions, non-throwing-by-contract transforms) from those docs are still
load-bearing and unchanged. If code contradicts this doc, the code wins —
re-verify.

---

## 1. The golden rule (new, foundational)

**Router chooses pipeline, based on data. Pipeline chooses action, based on
state.** Two separate decision axes, never crossed:

- **Pipeline = a self-contained *definition*** — a pure `(state) → action`
  lookup table, one file, fully readable top-to-bottom. A pipeline never
  calls itself again and never privately loops.
- **Router = the only *execution loop***. Every single action — including
  same-pipeline retries (self-loop on transient error) — goes
  `action → Router → pipeline lookup → action`. When an action's outcome
  doesn't change data shape (e.g. a transient DB error), the Router
  re-selects the *same* pipeline, which is functionally a self-loop but
  mechanically just the Router re-dispatching. When an action's outcome
  *does* change data shape, the Router naturally picks a different pipeline
  next. No pipeline ever owns its own continuation past a single action.

This closes the exact bug class the original router-gap audit found (states
falling through silently because a pipeline's internal loop and the router
disagreed about who owned dispatch) — there is no longer an "internal loop"
to disagree with.

**Not every state→action mapping is a pure 1:1 lookup.** Some states need a
secondary data check to pick the destination (e.g. `IdempotentInboxMessageState`
and `ValidateInboxMessageInvalidErrorState` both null `InboxMessage`, landing
in the same pipeline slot, but need different destinations inside it — resolved
per-case, not via a general mechanism).

---

## 2. Project structure — final (supersedes prior docs)

Four layers, strict DAG, each layer depends only on the layer immediately
below it:

```
Layer 1 — Persistence / Transport (base, no cross-references between them)
  Persistence.Inbox, Persistence.Outbox, Persistence.DeadLetter
  Transport.Envelope, Transport.DeadLetterEnvelope

Layer 2 — Operations (six projects — see §3 for why not five, not three)
  Operations.Inbound.Envelope
  Operations.Inbound.DeadLetterEnvelope
  Operations.Inbound.Inbox
  Operations.Inbound.DeadLetter
  Operations.Outbound.Envelope
  Operations.Outbound.Outbox

Layer 3 — Pipelines (by lifecycle group)
  Pipelines.Inbound, Pipelines.Outbound, Pipelines.DeadLetter

Layer 4 — Routing (by direction; decorators live here)
  Routing.Inbound   (spans Pipelines.Inbound + Pipelines.DeadLetter)
  Routing.Outbound  (spans Pipelines.Outbound only)
```

**Package model**: single base NuGet package `Messaging.InboxOutbox` contains
all four layers above, including Instrumentation/Resiliency as internal
projects referenced only by `Routing.*` (no separate shared package).
Specialized packages `Messaging.Kafka`, `Messaging.SqlServer`,
`Messaging.MongoDb` each reference `Messaging.InboxOutbox` and supply
broker/store-specific implementations. This **replaces** the prior 5-package
model (`Messaging.Shared` as a separate package is discarded).

**Decorators live in `Routing.*`** — the layer that actually *executes*
(calls operations), not the layer that merely *defines* them. Consistent with
the golden rule: Router is the sole execution authority, so it's the one
place all operation invocations funnel through and can be uniformly wrapped
for instrumentation/critical-exception classification.

---

## 3. Why six Operations projects, not three and not five

**First attempt (5 projects, by entity)** — mirrored the five Layer-1
projects one-to-one (`Operations.Envelope`, `Operations.DeadLetterEnvelope`,
`Operations.InboxMessage`, `Operations.OutboxMessage`,
`Operations.DeadLetterMessage`). This correctly fixed a real dependency-purity
problem (the original 3-project-by-message-type model had e.g.
`Operations.Inbox` pulling `Persistence.Inbox` even for operations like
Capturing/Confirming that never touch `InboxMessage` at all).

**But it broke on two things, discovered when actually splitting the code:**

1. **Straddling operations.** Every `Mapping` and every `Converting` operation
   in the library transforms one entity into another by definition (that's
   what those verbs do) — `MapEnvelope` (Envelope→InboxMessage),
   `ConvertEnvelope` (Envelope→DeadLetterEnvelope), `ConvertInboxMessage`
   (InboxMessage→DeadLetterMessage), `MapDeadLetterMessage`
   (DeadLetterMessage→DeadLetterEnvelope), `MapOutboxMessage`
   (OutboxMessage→Envelope). None of these fit cleanly into an entity-only
   project.
2. **`Envelope` and `DeadLetterEnvelope` mixed inbound and outbound
   operations in the same project** — e.g. `Operations.Envelope` would have
   held Inbox's Capturing/Confirming *and* Outbox's Publishing/Producing
   together, which don't belong together structurally (different directions,
   different pipelines, no reason to co-locate).

**Resolution — placement follows input entity, same rule as operation
naming.** Operations are already named for what they consume, not produce
(`ConvertEnvelope` takes `Envelope`, produces `DeadLetterEnvelope`, named for
the input). Applying that same rule to *project placement*, and crossing it
with direction (inbound/outbound) to fix the mixing problem, produces six
projects:

| Project | Contents (by input entity) |
|---|---|
| `Operations.Inbound.Envelope` | Capturing, Validating(envelope), Confirming, Mapping (input=Envelope), Converting (input=Envelope) |
| `Operations.Inbound.DeadLetterEnvelope` | Redirecting (input=DeadLetterEnvelope — moved here from Envelope; it only ever operates on the *converted* result, never touches Envelope itself), DL's Publishing, DL's Producing |
| `Operations.Inbound.Inbox` | Validating(InboxMessage), Inserting, Handling, Transacting, Abandoning, Scheduling, Converting (DL's — input=InboxMessage), Closing (new — see §5) |
| `Operations.Inbound.DeadLetter` | DL's Inserting, Mapping (DL's — input=DeadLetterMessage), Scheduling, Abandoning, Closing |
| `Operations.Outbound.Outbox` | Validating, Transacting, Mapping (input=OutboxMessage), Closing, Abandoning, Scheduling |
| `Operations.Outbound.Envelope` | Publishing, Producing (input=Envelope only) |

No sixth "boundary operation" category needed — every straddler resolves by
the same single rule (input entity + direction), no exceptions.

`Pipelines.DeadLetter` stays a standalone project (own lifecycle, own state
list) despite having no router of its own — it's triggered by an internal
status transition (`InboxMessage.Status == Abandoning`), not by anything
crossing the process boundary, so it doesn't fit "inbound" or "outbound"
as a *pipeline* grouping even though `Routing.Inbound` is the router that
dispatches into it.

---

## 4. Naming conventions confirmed/corrected this session

- **Pure-transform delegates use `From<InputType>`, not `To<OutputType>`** —
  matches the established sibling pattern (`FromEnvelope`,
  `FromEnvelopeValue`, `FromDeadLetterMessagePayload`,
  `FromOutboxMessagePayload`, etc.). The one outlier, DL's Converting
  transform, was renamed `ToDeadLetterMessage` → `FromInboxMessage` for
  consistency. Note this is a different naming layer than *operation* names
  (`ConvertEnvelope`, `MapEnvelope`) — operations are `Verb(input)` service
  calls with a different grammar than instance-style `To*` methods
  (`ToString()`), so operations correctly stay input-named without needing
  to match the `From*` delegate convention structurally — they already do,
  coincidentally, but for a different reason.
- **Router-layer naming settled as `Inbound`/`Outbound`**, not
  `Consuming`/`Producing` — community-adopted messaging vocabulary,
  broker-agnostic, consistent with the earlier `Offsetting` → `Confirming`
  rename (killed for the same reason: Kafka-specific terms don't generalize
  to RabbitMQ/ASB).
- **`CapturingNotStartedState`** — new pseudo-state, Inbound-pipeline-entry
  specific. Represents "just entered the Capturing pipeline, nothing's run
  yet," giving the Router something to switch on for the very first dispatch.

---

## 5. Capturing split into Capturing + Validating (Envelope)

Originally one operation (`CaptureEnvelope`) did both the broker read (I/O,
retryable) and the envelope-shape check (pure transform, non-retryable) in
one method. Split this session, mirroring the same conflation reasoning that
originally justified Handling → Handling/Transacting/Abandoning: a retryable
I/O concern and a non-retryable pure-transform concern were bundled, and
splitting makes the pattern visible as its own step (matches the existing
Mapping → Validating shape already used for `InboxMessage` construction).

**Capturing** (`Operations.Inbound.Envelope/Capturing`) — pure I/O, 3 states:
- `CaptureEnvelopeSuccessState`
- `NotCapturedEnvelopeState` (empty poll)
- `CaptureEnvelopeErrorState` (self-loop, transient)

**Validating** (`Operations.Inbound.Envelope/Validating`) — pure transform,
4 states:
- `ValidateEnvelopeSuccessState`
- `ValidateEnvelopeInvalidErrorState` — unconfirmable (no `Confirmation`
  present, nothing salvageable)
- `ValidateEnvelopeInvalidConfirmableErrorState` — confirmable
  (`Confirmation` present despite another field failing — e.g. Key/Value/
  Type/Metadata null) — decided **inside** `ValidateEnvelope` itself (not via
  a data-check in the pipeline switch, which would violate the golden rule:
  state alone must decide the action)
- `ValidateEnvelopeErrorState` (kept for catch-consistency with siblings,
  likely provably unreachable, same status as `ConvertEnvelopeErrorState`)

`ValidateEnvelope` only checks nullability (not deeper business validity) —
confirmed via the actual `ValidateEnvelope` implementation shared this
session (checks `Key`/`Value`/`Type`/`Metadata`/`Confirmation` for null only).

**Open/unconfirmed**: whether `ValidateEnvelopeInvalidConfirmableErrorState`
populates `data.Confirmation` alone while leaving `data.Envelope` null (so
the pipeline-gating shape — `Envelope` null-ness deciding Capturing vs.
Mapping pipeline — stays honest and doesn't mis-route a confirmable-but-
invalid envelope into the Mapping pipeline). Flagged, not yet resolved in
code.

---

## 6. DL Inserting split into Inserting + Closing (Inbox)

Previously one conceptual step; split into two **serial** (not transacted)
operations this session:

- **`InsertDeadLetterMessageAsync`** (`Operations.Inbound.DeadLetter`) —
  persists the `DeadLetterMessage`.
- **`CloseInboxMessageAsync`** (`Operations.Inbound.Inbox` — placed here, not
  DeadLetter, because its input entity is `InboxMessage`) — writes the
  `InboxMessage`'s terminal `Closed` status, reached only after the DL insert
  succeeds.

This gives Inbox a `Closing` operation it previously lacked (flagged as an
open gap in the prior handoff's pipeline sketch — now resolved, not an
oversight).

---

## 7. Status model — `InboxMessageStatus` updated, DL/Outbox unchanged

**`InboxMessageStatus`: `Processing`, `Handled`, `Abandoning`, `Closed`**
(was `Processing, Handled, DeadLettering, DeadLettered`).

Renamed because `DeadLettering`/`DeadLettered` named the *next entity*
(pointed at DL) rather than describing the Inbox message's own state — a
naming smell caught this session. New names follow a **tense-signals-
terminality rule**, confirmed by checking each status against one concrete
question — "does this status still owe a handoff?":

- `Processing` — non-terminal (in flight)
- `Handled` — terminal, past-tense (domain succeeded, via Transacting)
- `Abandoning` — **non-terminal, deliberately present-participle** — the gate
  into the DL pipeline. Both `HandleInboxMessageDomainErrorState` (domain
  rejection) and `ScheduleInboxMessageExhaustedState` (retry exhaustion) now
  route to this same status via `AbandonInboxMessageAsync` — a genuine
  architectural consolidation (previously domain-rejection wrote a terminal
  status directly with no DL involvement; now both causes funnel through DL
  uniformly, no more silent-discard path).
- `Closed` — terminal, past-tense, reached via
  `InsertDeadLetterMessageAsync → CloseInboxMessageAsync` (§6).

**`DeadLetterMessageStatus` and `OutboxMessageStatus` — confirmed unchanged,
`Processing / Published / Abandoned`.** Checked against the same tense rule
and found already correct: both subsystems' `Abandoned` is genuinely
terminal (DL has nowhere further to hand off to; Outbox dead-lettering was
ruled out permanently in an earlier session) — no present-participle needed.
Both already have the same dual-route-convergence shape as Inbox
(`ScheduleXMessageExhaustedState` writes `Abandoned` inline as part of its
own atomic write, *and* `AbandonXMessageAsync` — reached from Mapping's
failure path — writes it independently) — the consolidation pattern already
existed here, it just didn't need a tense change because neither route owes
a further handoff the way Inbox's does.

**Status model is otherwise still closed** per the 2026-08-25 Outbox doc: one
non-terminal `Processing` per subsystem, no `Pending` anywhere, no further
status-splitting planned.

---

## 8. Full current operation inventory (verbatim from code, 2026-08-26)

### Operations.Inbound.Envelope
- Capturing — `CaptureEnvelopeSuccessState`, `NotCapturedEnvelopeState`, `CaptureEnvelopeErrorState`
- Validating — `ValidateEnvelopeSuccessState`, `ValidateEnvelopeInvalidErrorState`, `ValidateEnvelopeInvalidConfirmableErrorState`, `ValidateEnvelopeErrorState`
- Confirming — `ConfirmEnvelopeSuccessState`, `ConfirmEnvelopeErrorState`, `ConfirmEnvelopeCriticalErrorState`
- Converting — `ConvertEnvelopeSuccessState`, `ConvertEnvelopeInvalidState`, `ConvertEnvelopeErrorState`
- Mapping — `MapEnvelopeSuccessState`, `MapEnvelopeErrorState`, `MapEnvelopeValueErrorState`

### Operations.Inbound.DeadLetterEnvelope
- Redirecting — `RedirectDeadLetterEnvelopeSuccessState`, `RedirectDeadLetterEnvelopeCircuitOpenState`, `RedirectDeadLetterEnvelopeErrorState`
- Publishing — `PublishDeadLetterEnvelopeSuccessState`, `PublishDeadLetterEnvelopeErrorState`, `PublishDeadLetterEnvelopeCriticalErrorState`
- Producing — `ProducingDeadLetterEnvelopeState`, `ProduceDeadLetterEnvelopeErrorState`, `ProduceDeadLetterEnvelopeCriticalErrorState`

### Operations.Inbound.Inbox
- Validating — `ValidateInboxMessageSuccessState`, `ValidateInboxMessageErrorState`, `ValidateInboxMessageInvalidErrorState`
- Inserting — `InsertInboxMessageSuccessState`, `InsertInboxMessageErrorState`, `InsertInboxMessageCircuitOpenState`, `IdempotentInboxMessageState`
- Handling — `HandleInboxMessageSuccessState`, `HandleInboxMessageDomainErrorState`, `HandleInboxMessageTechnicalErrorState`
- Transacting — `TransactInboxMessageSuccessState`, `TransactInboxMessageErrorState`
- Abandoning — `AbandonInboxMessageSuccessState`, `AbandonInboxMessageErrorState`, `AbandonInboxMessageCircuitOpenState`
- Scheduling — `ScheduleInboxMessageExhaustedState`, `ScheduleInboxMessageRetryState`, `ScheduleInboxMessageErrorState`
- Converting (DL's, input=InboxMessage) — `ConvertInboxMessageSuccessState`, `ConvertInboxMessageErrorState`
- Closing (new, §6) — `CloseInboxMessageSuccessState`, `CloseInboxMessageErrorState`, `CloseInboxMessageCircuitOpenState`

### Operations.Inbound.DeadLetter
- Inserting — `InsertDeadLetterMessageSuccessState`, `InsertDeadLetterMessageErrorState`, `IdempotentDeadLetterMessageState`
- Mapping — `MapDeadLetterMessageSuccessState`, `MapDeadLetterMessageErrorState`, `MapDeadLetterMessagePayloadErrorState`
- Scheduling — `ScheduleDeadLetterMessageSuccessState`, `ScheduleDeadLetterMessageExhaustedState`, `ScheduleDeadLetterMessageRetryState`, `ScheduleDeadLetterMessageErrorState`
- Abandoning — `AbandonDeadLetterMessageSuccessState`, `AbandonDeadLetterMessageErrorState`
- Closing — `CloseDeadLetterMessageSuccessState`, `CloseDeadLetterMessageErrorState`

### Operations.Outbound.Outbox
- Validating — `ValidateOutboxMessageSuccessState`, `ValidateOutboxMessageErrorState`, `ValidateOutboxMessageInvalidErrorState`
- Transacting — `TransactOutboxMessageSuccessState`, `TransactOutboxMessageErrorState`, `IdempotentOutboxMessageState`
- Mapping — `MapOutboxMessageSuccessState`, `MapOutboxMessageErrorState`, `MapOutboxMessagePayloadErrorState`
- Closing — `CloseOutboxMessageSuccessState`, `CloseOutboxMessageErrorState`
- Abandoning — `AbandonOutboxMessageSuccessState`, `AbandonOutboxMessageErrorState`
- Scheduling — `ScheduleOutboxMessageExhaustedState`, `ScheduleOutboxMessageRetryState`, `ScheduleOutboxMessageErrorState`

### Operations.Outbound.Envelope
- Publishing — `PublishEnvelopeSuccessState`, `PublishEnvelopeErrorState`, `PublishEnvelopeCriticalErrorState`
- Producing — `ProducingEnvelopeState`, `ProduceEnvelopeErrorState`, `ProduceEnvelopeCriticalErrorState`

---

## 9. Pipeline sketching — in progress, not settled

Started sketching `Pipelines.Inbound`'s Capturing entry as a state→action
table this session, but it predates the Capturing/Validating split and the
`Confirmation`-check resolution (§5) — **the draft sketch from earlier in
this session is now stale and needs to be redrawn** against the current
operation inventory in §8 before it's usable. Not attempted yet for Mapping,
Inserting, Handling, or the DL/Outbox pipelines.

**Resolved this session, ready to apply when pipelines are redrawn:**
- `HandleInboxMessageTechnicalErrorState` — represents a technical failure
  during Handling (e.g. an unhandled DB exception), **not** a domain
  rejection. Should route as a **self-loop retry** (same DB-write-only /
  transient-failure treatment as other technical errors), **not** to
  Abandoning — routing it to Abandoning would conflate a transient
  operational failure with a genuine domain rejection, the same category of
  bug the original Redirect/Convert split was created to avoid.
- Both `HandleInboxMessageDomainErrorState` and
  `ScheduleInboxMessageExhaustedState` route to `AbandonInboxMessageAsync`
  (§7 — confirmed consolidation).
- `IdempotentInboxMessageState` routes directly to `ConfirmEnvelope`,
  bypassing Converting (confirmed — an idempotent duplicate isn't a bad
  message, no dead-lettering needed).
- `ValidateInboxMessageInvalidErrorState` / `ValidateInboxMessageErrorState`
  both null `InboxMessage` and route to the Mapping pipeline's Converting
  step (dead-letter path — a genuinely bad message).

---

## 10. Still open / not yet done

- **Pipeline definitions** — redraw Capturing (stale draft, §9), then Mapping,
  Inserting, Handling for `Pipelines.Inbound`; DL's pipeline for
  `Pipelines.DeadLetter`; not yet started for `Pipelines.Outbound`.
- **`ValidateEnvelopeInvalidConfirmableErrorState`'s data contract** — does it
  set `data.Confirmation` alone, leaving `data.Envelope` null? (§5, flagged
  not yet resolved in code.)
- **Router wiring** — no pipeline has been threaded through actual router
  switch statements yet under the new golden-rule model.
- **Produce/Publish DI seam** — clarified in direction this session (router
  branches to Publishing or Producing based on config; both operations live
  in DI simultaneously, broker's transport functions satisfy both shapes) but
  the actual interface shape is still not designed.
- **Broker capability verification** — still unverified against real
  Kafka/RabbitMQ/ASB client APIs (carried forward, unchanged). Noted this
  session: every broker has at least sync Publishing; default pipelines may
  end up broker-parameterized — explicitly parked for later discussion.
- **`DeadLetterMessage.LastError` field** — confirmed exists (`string?`),
  closed.
- **Dynamic pipeline chaining** — still parked; confirmed this session to be
  a Router/Routing-layer responsibility (not a pipeline-level concern), to be
  designed once routing itself is implemented.
- **Redelivering pipeline** — still parked, unresolved, carried forward
  unchanged.

---

## 11. How to use this doc

Load alongside the three 2026-08-25 docs for operation-level conventions
(return shape, guard discipline, `PipelineError`, exception handling) — those
are still current. Treat **this doc's project-structure sections (§2–3) as
superseding** the prior docs' 5-package/3-project model entirely. Treat §4–8
as settled, verified-against-code state. Treat §9–10 as the active task list
— most likely next step is redrawing the Capturing pipeline against the
current (post-split) operation inventory, then working through Mapping,
Inserting, and Handling in the same session before moving to
`Pipelines.DeadLetter`. As always: if code contradicts this doc, the code
wins — re-verify before relying on anything here.
