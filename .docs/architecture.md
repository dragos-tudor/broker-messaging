
## Architecture [v1]
- core libraries.
- specialized libraries for transport (Kafka, RabbitMQ, Azure Service Bus).
- specialized libraries for persistence (SQL Server, MongoDb).
- specialized libraries will refer ONLY core library.

## Features
- consume, persist and handle inbound messages.
- persist and publish outbound messages.

## Pipelines
- inbound pipeline for incoming broker messages.
- outbound pipeline for outgoing broker messages.

### Pipelines Types
- inbound pipeline contains 4 distinct types:
  - envelope [transport].
  - inbox message [persistence].
  - dead letter message [persistence].
  - dead letter envelope [transport].
- outbound pipeline contains 2 distinct types:
  - outbox message [persistence].
  - envelope [transport].

### Pipelines Segments
- inbound pipeline segments:
  - envelope [for incoming messages].
  - inbox message [from envelopes].
  - dead letter message [from broken envelopes or inbox messages].
  - dead letter envelope [from dead letter message].
- outbound pipeline segments:
  - outbox message [from developers].
  - envelope [from outbox messages].

### Pipelines Operations [Mirror Pipelines Segments]
- inbound pipeline operations:
  - envelope operations.
  - inbox message operations.
  - dead letter message operations.
  - dead letter envelope operations.
- outbound pipeline operations:
  - outbox message operations.
  - envelope operations.
- each operation -> one-task responsibility (capturing, inserting, handling, mapping).
- each operation is independent of the others.
- each operation uses specialized interfaces for services and data based on composition root pattern.

## Routers
- inbound router orchestrates inbound pipelines executing operations.
- outbound router orchestrates outbound pipelines executing operations.
- IMPORTANT:
  - router → decides what executes next.
  - pipeline → defines/executes a processing flow.
  - operation → performs one processing task.

## Libraries
- broker-agnostic client messaging library at the core.
- specialized persistence messages libraries.
- specialized transport envelopes libraries.
- a complete client is composed from:
  - the core library.
  - one specialized persistence library.
  - one specialized transport library.
- library projects form a DAG.

### Core Library
- foundation layer: persistence.* and transport.* projects.
- operations layer: operations.* projects.
- pipelines layer: pipelines.* project.
- routing layer: routing.* projects.
- system layer: resiliency.* and observability.* projects.
- meta layer: messaging.core meta-project.

### Specialized Libraries
- foundation layer: kafka.* projects.
- meta layer: messaging.kafka meta-project.

### Golden rules
- all projects on a layer may reference only projects from the layer immediately below.
- all projects within the same layer are independent of each other.
- all projects should use high cohesion internally, minimal coupling externally.
- broker-specific types and concepts must not leak into the core library.

## Packages
- each meta-project is packed as NuGet package.
- meta-projects are composition/packaging boundaries and contain no implementation.
- messaging.core package packs all core project assemblies.
- messaging.kafka package packs all kafka package assemblies.

## Main Patterns
- implement transactional inbox pattern for inbound messages.
- implement transactional outbox pattern for outbound messages.
- at-least-once strategy for inbound and outbound messages.
- ensure idempotency for inbound messages.
- composition root for services and data interfaces.
- orchestrator pattern for pipelines.
- fractal architectural pattern:
  - independent modules form the foundation at each level of composition.
  - higher layers compose them into progressively more capable modules.
  - the same principle applies recursively from functions to projects and libraries.