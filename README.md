# Broker Messaging

## Broker agnostic messaging client library

A high-throughput, **transactional inbox/outbox** messaging library as broker-backed systems. Designed around a strict multi-layer directed acyclic graph (DAG), specialized pipeline step operations and an orchestrator pattern that keeps pipelines pure and readable.

(*ongoing architecture, design, decisions, code-conventions docs on for AI [docs](./.docs)*).

---

## What the project provides

Broker Messaging models the full lifecycle of messages moving between an application, persistent storage, and a message broker.

It supports two main directions:

- **Inbound** — messages arriving from a broker and entering the application.
- **Outbound** — messages produced by the application and eventually published to a broker.

Across both directions, the project provides reusable concepts for:

- message capture and persistence
- validation and mapping
- application handling
- publishing and dispatching
- retries and delayed recovery
- dead-letter processing
- idempotent execution
- final acknowledgement or abandonment

The emphasis is not on hiding these steps behind a large abstraction. The emphasis is on making them visible, composable, and easy to reason about.

---

## Core design

### Independent operations

The smallest executable units are **operations**.

Each operation has one responsibility and represents one meaningful action in the message lifecycle.

Examples include capturing a broker message, validating data, inserting a persistent message, publishing an envelope, closing a message, or scheduling another attempt.

Operations are intentionally kept independent from complete workflows. The same operation may participate in multiple pipelines without being owned by any single one.

This keeps the system reusable and prevents workflow-specific concerns from leaking into low-level behavior.

---

### Pipelines as explicit flow segments

Operations are combined into **pipelines**.

A pipeline does not implement business behavior itself. It defines how processing moves from one operation to the next based on the result of the previous step.

The pipelines are split into natural flow segments such as:

- capturing
- redirecting
- handling
- dead-lettering
- persisting
- publishing
- dispatching

This segmentation keeps individual flows small enough to understand and test independently while still allowing them to compose into a larger message lifecycle.

The result is closer to a visible state graph than to a hidden orchestration framework.

---

### Routers and runners

Pipelines describe possible transitions. **Routers** execute those transitions.

A router selects the current operation, executes it, interprets the result, and decides the next action.

A **runner** sits outside the router and is responsible for restarting processing when longer recovery is required.

This separation keeps two different concerns distinct:

- short-term execution and routing
- long-term recovery and restart

That distinction is important because not every failure should be treated the same way.

---

## Message structures

The project keeps transport-facing data separate from persistence-facing data.

### Transport structures

- **Envelope** — a broker-facing message representation.
- **Dead-letter envelope** — a broker-facing representation used when forwarding failed messages.

### Persistence structures

- **Inbox message** — persisted inbound work.
- **Dead-letter message** — persisted failed inbound work.
- **Outbox message** — persisted outbound work.

These structures are separate because they represent different concepts, lifecycles, and responsibilities.

The goal is to avoid using one generic message model for unrelated concerns.

---

## Reliability model

Reliability is built into the architecture rather than added around it.

### Idempotency

Message handlers and persistence operations are expected to behave safely when the same work is observed more than once.

This allows the system to recover from duplicate delivery, retries, and restarts without depending on exactly-once transport guarantees.

### Two recovery horizons

The project distinguishes between two kinds of retry behavior:

1. **Immediate retry** for short-lived failures during active processing.
2. **Scheduled recovery** for failures that require persistence and a later attempt.

This avoids turning every failure into the same retry loop.

Immediate retry belongs close to execution. Scheduled recovery belongs to persisted message state.

### Dead-letter processing

Messages that cannot continue through the normal path can be converted into dead-letter data and processed through their own explicit lifecycle.

Dead-letter handling is treated as a first-class flow, not as an exceptional side path hidden inside unrelated code.

---

## Architecture

The solution is organized as a dependency graph of small projects.

At the base are independent core concepts. More specialized projects build on them without creating circular ownership.

Typical layers include:

- foundational structures and shared abstractions
- reusable operations
- pipeline definitions
- routing and execution
- transport-specific integrations
- persistence-specific integrations
- application composition

Transport implementations such as Kafka and persistence implementations such as MongoDB or SQL Server depend on the shared core rather than on each other.

This allows new integrations to be added without changing the central model.

The architectural preference is:

> **independence first, composition second**

That keeps the dependency graph simple, enables parallel development and build execution, and makes replacement of individual parts easier.

---

## Design principles

The project follows a small set of rules throughout the codebase:

- **One operation, one responsibility**
- **Explicit flow instead of hidden orchestration**
- **Independent modules before composition**
- **Pure decision logic where possible**
- **Side effects isolated at the edges**
- **Idempotent message processing**
- **Domain concepts represented separately**
- **Retries handled centrally rather than duplicated**
- **Transport and persistence remain replaceable**

The intent is to keep the system understandable even as reliability requirements increase.

---

## Why this design

Messaging systems easily accumulate accidental complexity.

Retries, acknowledgements, persistence, broker callbacks, duplicate delivery, dead letters, and failure recovery can become deeply intertwined.

Broker Messaging tries to prevent that by separating:

- **what an operation does**
- **how operations are connected**
- **how execution is routed**
- **how long-running recovery is restarted**
- **how transport and persistence are implemented**

The result is a system where each concept can remain small while the full behavior emerges from composition.

---

## Project direction

The project is currently centered on a broker-agnostic core with specialized integrations built around it.

Kafka is the primary transport target, MongoDB as primary persistence target while the architecture is intended to support additional brokers and persistence technologies without changing the central processing model.

The broader objective is not to create another large messaging framework.

It is to provide a compact set of composable building blocks for reliable messaging while keeping the execution model visible to the developer.

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
---

## AI Credits
- Architecture/Design sessions [web + live conversations]:
  - GPT-5.6 Sol [Medium/High].
  - Sonnet 5 [Thinking].
- Implementation plan and code generation:
  - GPT-5.6 Luna [Medium].
  - Sonnet 5 [Medium].
  - Gemini 3.7 Flash [Medium].
- AI harnesses:
  - Codex VSCode extension.
  - Google Antigravity VSCode extension.
  - Github Copilot VSCode integration.

---
