
# Architecture

## Pyramidal DAG architecture

The system is organized as a directed acyclic graph of small, independently useful projects. Dependencies flow upward toward composition; projects at the same level do not depend on each other and should not require a shared horizontal/base project merely to cooperate.

This was chosen to preserve **independence before composition**. A component should remain understandable, buildable, testable, replaceable, and reusable without knowing the components that will eventually be composed with it.

Higher layers create capability by combining lower-level pieces rather than by moving common behavior into increasingly broad shared abstractions.

This structure also has practical consequences: independent branches can build in parallel, changes invalidate a smaller part of the dependency graph, and failures remain localized.

**Rejected direction:** layered architectures where sibling modules depend on one another or where broadly shared projects gradually become dependency hubs.

---

## Stable dependency direction over convenience

Project ownership and dependencies follow stable concepts, not whichever caller currently uses a component.

Transport-specific and persistence-specific implementations depend toward the corresponding abstractions/core structures. They do not become dependencies of unrelated components and do not depend on one another merely because they participate in the same runtime flow.

This keeps Kafka, RabbitMQ, SQL Server, MongoDB, and other specializations replaceable without propagating their concepts through the system.

**Decision rule:** dependencies represent conceptual ownership, not execution order.

---

## Composition creates the system

Individual projects intentionally provide incomplete capabilities. Complete broker-messaging behavior emerges only at higher composition levels.

This avoids turning low-level components into miniature versions of the whole application and keeps their responsibilities narrow enough to remain reusable.

The architecture therefore favors many simple pieces with explicit composition over fewer highly configurable components.

---

## Functional core with explicit impure edges

Behavior is expressed primarily as functions operating over data. I/O and other side effects are exposed through explicit service capabilities rather than hidden behind behavior-rich objects.

The purpose is not functional style for its own sake. It makes dependencies visible, keeps deterministic logic separate from external effects, and makes individual pieces easier to reason about and test.

The architecture therefore avoids introducing object graphs, inheritance hierarchies, or service objects merely to carry behavior.

---

## Orchestration and execution lifetime are separate concerns

The **Router** and **Runner** exist for different reasons and remain separate.

The Router performs bounded orchestration: identify the pipeline/action, execute an operation, interpret its result, apply short retry policy where appropriate, and decide what should happen next.

The Runner owns the longer execution lifetime and can restart routing after long backoff or other external recovery.

This prevents temporal/recovery concerns from turning the Router into an indefinitely running state machine.

---

## Reliability is part of the protocol, not invisible infrastructure

Retry, scheduling, dead-lettering, idempotency, confirmation, abandonment, and closing are represented explicitly where they affect business or persistence state.

They are not hidden entirely inside generic resilience middleware.

Invisible retry is appropriate only for short-lived repetition of the same operation. Once recovery changes durable state or future execution, it becomes an explicit part of the messaging protocol.

This keeps failure behavior observable, testable, and reconstructable from persisted data.

# Structures

## Different lifecycle concepts have different structures

`Envelope` and `DeadLetterEnvelope` are separate structures.

`InboxMessage`, `DeadLetterMessage`, and `OutboxMessage` are also separate structures.

They may contain similar data, but they represent different concepts with different lifecycles, invariants, operations, persistence requirements, and valid transitions.

A single generalized message structure would save types at the cost of merging concepts and allowing combinations of state that have no valid meaning.

**Decision rule:** structural similarity is not sufficient reason to merge concepts.

---

## Transport and persistence representations remain distinct

Broker-facing structures and persisted messaging structures model different boundaries.

An envelope represents transport information and broker interaction. Inbox, outbox, and dead-letter messages represent durable processing state.

The transition between them is explicit through mapping/conversion operations rather than being hidden by using one universal message model.

This prevents persistence concerns from leaking into transports and transport concerns from defining the durable model.

---

## Dead-letter data is a first-class model

Dead-letter envelopes and dead-letter messages are not treated as ordinary envelopes/messages carrying an error flag.

Dead-letter processing has its own lifecycle: conversion, persistence, publishing or dispatching, scheduling, abandonment, and closing.

Representing that explicitly avoids forcing the normal processing model to contain states and fields that exist only because processing has failed.

---

## Structures carry data; operations carry behavior

Core structures remain primarily data representations. Behavior belongs to operations/functions rather than being attached to structures through instance methods.

This allows the same data to participate in different flows without giving the structure knowledge of those flows and supports the architectural separation between stable data concepts and composable behavior.

# Operations

## One operation represents one semantic task

An operation performs one meaningful task: capture, verify, map, validate, insert, confirm, handle, transact, schedule, publish, close, and so on.

Operations are intentionally smaller than use cases or pipelines.

This gives each operation one reason to exist, one result vocabulary, and a clear side-effect boundary. It also makes operations independently reusable and allows pipelines to express workflows by composition rather than by embedding workflow logic inside operations.

**Rejected direction:** multifunction operations whose behavior depends on the pipeline or execution context that called them.

---

## Operations belong to direction and entity, not to pipelines

Operations are organized around stable ownership such as inbound/outbound and the entity they operate on.

They are not owned by individual pipelines.

The reason is structural: the same operation can legitimately participate in multiple pipelines. Pipeline-based ownership would either duplicate the operation, introduce artificial cross-pipeline dependencies, or make one pipeline appear to own behavior that is actually shared.

Pipelines compose operations; they do not define their ownership.

---

## Operation execution data may be mutated locally to avoid allocation

The operation execution path is performance-sensitive. Where several operations need to evolve the same execution context, the existing operation data can be updated rather than allocating a new carrier object for every step.

This is a deliberate, constrained form of mutation whose purpose is to avoid unnecessary heap allocations and copying during orchestration.

It does **not** imply a move toward behavior-rich mutable domain objects. Mutation remains local to the execution mechanism and does not change the conceptual separation between data and behavior.

---

## Operation outcomes preserve their exact semantic type

Different operations have different result/state vocabularies. Those distinctions are preserved through operation-specific enum/result types rather than collapsing them into `string`, `Enum`, or another weak common representation.

This provides compile-time separation between concepts, avoids accidental acceptance of states belonging to another operation, and avoids boxing merely to obtain a common runtime type.

The orchestration mechanism must adapt to those precise types rather than weakening the domain model to make orchestration easier.

**Decision rule:** infrastructure adapts to semantics; semantics are not weakened for infrastructure convenience.

---

## Domain continuation and technical failure remain different channels

Expected domain outcomes are represented explicitly by operation results/states because they influence normal workflow continuation.

Exceptions represent failures of execution or infrastructure rather than alternative domain branches.

Keeping these channels separate prevents normal control flow from being expressed through exceptions and prevents technical failures from being disguised as ordinary domain outcomes.

---

## Side effects are supplied as capabilities

Operations do not acquire infrastructure implicitly. Required side effects are supplied through narrow service capabilities/delegates.

This keeps dependencies visible and allows operations to retain a functional shape even when the operation itself performs I/O.

Static callbacks and explicit parameters are preferred where they also avoid unnecessary closures and allocations.

# Pipelines

## Pipelines describe continuation; operations perform work

A pipeline does not implement the operation.

Its responsibility is to answer:

> Given the outcome of the current operation, what happens next?

This separates execution from workflow topology.

The resulting pipeline functions remain small, deterministic, and readable because they describe only transitions between operations, pipeline segments, and terminal outcomes.

---

## Pipelines are divided at natural flow boundaries

The complete broker workflow is intentionally not represented as one giant pipeline/state machine.

It is divided into segments such as capturing, redirecting, handling, dead-lettering, persisting, publishing, and dispatching where the flow naturally changes responsibility, data, or recovery semantics.

These boundaries are not chosen per entity because it wouldn't be naturally. They represent meaningful transitions in the fluent processing lifecycle.

A pipeline segment should therefore correspond to a coherent flow that can be understood independently while still composing naturally with the next segment.

**Rejected direction:** arbitrary segmentation by entity or one global state machine containing every possible messaging path.

---

## Pipeline topology is isolated for readability and exhaustive testing

Transition selection is isolated from operation execution so that the workflow graph can be inspected and tested directly.

Because a pipeline transition is essentially a deterministic mapping from an operation outcome to the next step, every valid branch can be enumerated without executing brokers, databases, handlers, or resilience mechanisms.

This makes pipeline tests tests of the actual workflow topology rather than indirect integration tests.

The isolation exists primarily for **human readability and deterministic verification**, not merely code organization.

---

## Pipeline concepts remain distinct types

Pipeline identifiers, operation actions, operation outcomes/states, and terminal actions are different concepts and must not be collapsed simply because the orchestrator needs to transport all of them.

The previous use of a common `string` representation made unrelated concepts interchangeable. Using `Enum` as a common abstraction would preserve the same conceptual problem while also introducing boxing.

The type system should preserve these boundaries, even when doing so makes the Router implementation more sophisticated.

---

## Fast retry does not belong to pipeline topology

Short retries repeat the same operation and do not represent a new semantic step in the workflow.

They therefore belong to Router/resilience execution rather than being modeled as pipeline transitions.

Only after retry finishes does the pipeline observe the final operation outcome.

This keeps the pipeline focused on semantic continuation instead of execution mechanics.

---

## Durable retry is an explicit pipeline concern

Scheduling differs fundamentally from fast retry because it changes durable message state and intentionally postpones processing.

Scheduling therefore appears as an explicit operation/flow where appropriate.

This distinction gives the system two separate mechanisms:

* **fast retry** — repeat the current non-durable operation within the current execution;
* **scheduling** — persist retry information and allow processing to resume later.

Treating these as the same mechanism would mix execution resilience with durable messaging semantics.

---

## Pipelines express the conceptual structure of the workflow

Pipeline segmentation, operation names, states, actions, and transitions should make the messaging protocol visible in code.

The goal is not to construct the smallest possible state-machine implementation. The goal is for a reader to reconstruct the processing model directly from its structure.

When implementation convenience conflicts with preserving those concepts, preserving the conceptual model takes precedence.

# Most Important decisions
- independence before composition.
- projects are organized by responsibility, not by workflow.
- different lifecycles deserve different types.
- operations preserve semantic precision.
- pipelines expose workflow topology rather than hiding it.
