## Design Note: Naming as an Iterative Revealing Process

The project structure and operation naming in this library — particularly the split between `Kafka.Operations.Inbox` and `Kafka.Outbox`, and the vocabulary used for retry/recovery operations — wasn't arrived at in a single pass. It came out of an iterative back-and-forth between the author and an AI assistant (Claude), and is documented here deliberately, because the *process* is as instructive as the *result* for anyone studying this codebase.

### Starting point

The library began organized around four state machines — consuming, resuming, publishing, relaying — each backed by operations grouped by feature inside a single `Kafka.Operations` project (folders: capturing, inserting, handling, scheduling, publishing, etc.). This surfaced a real problem: several operations across different state machines wanted the same name. Most collisions were legitimate (the same conceptual operation, shared on purpose), but a few weren't — "scheduling" was being used for three genuinely different things: retry-backoff on inbox messages, retry-backoff on outbox messages, and dead-letter dispatch scheduling.

### First attempt: solve it in place

The first instinct was to keep the single `Kafka.Operations` project and just find better names — no restructuring, just better words. This got partial results:

- "Publishing" and "scheduling" dead letters, shared across resuming and relaying, were confirmed as genuinely fine to share — same operation, same name, no problem.
- "Recovering" (re-handling inbox messages) and "redelivering" (re-publishing outbox messages) were kept as deliberate alternatives to the more literal "rehandling" / "republishing," which read awkwardly as English. These held up through most of the session.
- The unresolved case was "scheduling" itself: it was being used for three different things (inbox retry backoff, outbox retry backoff, dead-letter dispatch scheduling), and no single-project renaming attempt — including candidates like "deferring" and "postponing" — fully separated all three without the names starting to feel arbitrary or hard to remember.

This attempt clarified the actual problem: it wasn't that the *words* were bad, it was that a single flat project was asking three unrelated concepts to share one name. That realization is what motivated stepping back to a structural question rather than continuing to search for a fourth word.

### False starts, kept for the reasoning

A few structural options were explored and rejected, each for a specific, documented reason rather than vague dissatisfaction:

- **Split projects one-per-state-machine** (`Kafka.Consuming`, `Kafka.Resuming`, etc.) — solved the naming collision by namespace isolation, but introduced a cross-project dependency problem: resuming's "recovering" operation is really a wrapper around consuming's "handling" operation, which would have forced `Kafka.Resuming` to depend on `Kafka.Consuming`. Workable, but architecturally messier than necessary.
- **A dedicated `Kafka.DeadLettering` project** shared by resuming and relaying — solved the dependency problem for dead letters specifically, but introduced a third organizing axis (message-type-agnostic) cutting across a codebase that was otherwise organized by pipeline. Rejected on taste, not correctness.

### The insight that unlocked the shape

The breakthrough wasn't a new naming scheme — it was noticing that the four-state-machine view had displaced an earlier, simpler mental model: **there are only two pipelines here, incoming and outgoing. (author)** The move to four flows (consuming/resuming/publishing/relaying) was necessary for implementing the state machines, but it wasn't the right axis for organizing the *operations* underneath them.

Splitting `Kafka.Operations` into **`Kafka.Operations.Inbox`** and **`Kafka.Outbox`** — grouping by which pattern-half an operation belongs to, transactional inbox or transactional outbox — resolved every open problem at once:
- Naming collisions disappeared, since inbox-side and outbox-side operations are naturally distinct namespaces.
- The wrapper dependency problem disappeared, since e.g. handling and its retry variant now live in the *same* project.
- No third axis was needed for dead letters — inbox dead letters live with inbox operations, outbox dead letters live with outbox operations.
- The names double as documentation: anyone familiar with the transactional inbox/outbox pattern recognizes the project structure immediately.

### Final operation vocabulary

**`Kafka.Operations.Inbox`**

| Operation | Meaning | Used by |
|---|---|---|
| `Capturing` | capturing a consumed Kafka message | consuming |
| `Inserting` | inserting the inbox message | consuming |
| `Offsetting` | offsetting the topic partition | consuming |
| `Handling` | handling the inbox message, including transient-failure handling | consuming, resuming |
| `Dispatching` | dispatching an inbox message to the dead-letter flow | consuming, resuming |
| `Scheduling` | setting the next retry attempt, owns retry-count bookkeeping | resuming |
| `Delaying` | deferring dispatch of an inbox dead letter | resuming |

**`Kafka.Outbox`**

| Operation | Meaning | Used by |
|---|---|---|
| `Publishing` | publishing the outbox message, including transient-failure handling | publishing, relaying |
| `Dispatching` | dispatching an outbox message to the dead-letter flow | publishing, relaying |
| `Scheduling` | setting the next retry attempt, owns retry-count bookkeeping | relaying |
| `Delaying` | deferring dispatch of an outbox dead letter | relaying |

Each operation lives in its own folder (`Handling/`, `Dispatching/`, `Scheduling/`, etc.), one folder per name, in both projects.

Same names, same shapes, in both projects — disambiguated entirely by which project you're standing in, which mirrors the fact that the underlying message types are structurally identical across inbox and outbox even though the implementations are not shared.

### One more collision, found late

Even after the inbox/outbox split, one collision slipped through: in `Kafka.Outbox`, the primary operation (`Publishing` the outbox message) and the dead-letter operation (`Publishing` the dead letter) shared the same name inside the same project — the exact kind of problem the split was meant to eliminate. `Kafka.Operations.Inbox` didn't have this issue, since its primary operation is `Handling`, not `Publishing`.

The dead-letter operation was renamed to `Dispatching` in both projects — not just Outbox, where the collision actually was. `Kafka.Operations.Inbox` didn't strictly need the rename, but keeping `Dispatching` (dead letter) consistent across both projects avoids a reader wondering whether `Publishing` (dead letter) in one project and a differently-named operation in the other represent the same concept. This mirrors the earlier decision to unify `deferring`/`postponing` into a single `Delaying`: once a project-local fix is needed, symmetry across the sibling project is worth the small unforced rename.

### From wrapper to merge: *Retrying* disappears

The vocabulary went through one more shift after the collision above was fixed. At that point, retrying a failed operation was modeled as a *separate* operation — `Retrying` — that wrapped the primary operation (`Handling` or `Publishing`) in a try/catch, colocated in the same folder specifically to avoid reintroducing a naming collision (a top-level `Retrying.cs` shared across folders would have collided with itself the same way `Scheduling` originally did). That was a reasonable model, but it implied two callable operations per primary action, and every state machine that needed the retry-capable version had to know to call `Retrying` instead of `Handling` directly.

The simplification: fold the transient-failure handling directly into `Handling` and `Publishing` themselves, so there's only ever one operation to call, and it's opaque about who's calling it or which attempt this is. This has a real consequence for how state machines share operations:

- **Consuming** calls `Handling` on the first attempt; **resuming** calls the exact same `Handling` on a later attempt. Neither state machine needs a different code path — `Handling` doesn't know or care which one is calling it or what attempt number this is.
- The only place attempt-count / backoff bookkeeping lives is `Scheduling`, which is resuming/relaying-only and entirely separate from the operations it schedules.

This removed `Retrying` as a named operation entirely, and with it the last bit of indirection between "the operation" and "the retry-safe version of the operation" — there's only one version now, and it's shared directly across state machines with no wrapping and no cross-project dependency.

### Why this is worth documenting

*The value of writing this down is the same lesson that ended the design session: naming problems are rarely solved by finding a better word — they're solved by finding the right \*boundary\*, and once the boundary is right, the words tend to follow naturally with very little effort.*