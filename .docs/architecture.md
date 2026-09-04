
## Architecture [v1]
- core libraries.
- specialized libraries for transport (Kafka, RabbitMQ, Azure Service Bus).
- specialized libraries for persistence (SQL Server, MongoDb).
- specialized libraries may reference ONLY the core library.

## Main Components
- foundation core structures.
- operations.
- pipelines.
- routers.

## Foundation Core Structures
- transport:
  - envelope broker-agnostic.
  - dead letter envelope broker-agnostic.
- persistence:
  - inbox message.
  - dead letter message.
  - outbox message.
  - retry plan.

## Operations [Mirror Pipelines]
- operation → performs one processing task.
- inbound:
  - envelope operations.
  - inbox message operations.
  - dead letter message operations.
  - dead letter envelope operations.
- outbound:
  - outbox message operations.
  - envelope operations.

## Pipelines
- pipeline: defines a segment processing flow.
- inbound pipeline segments:
  - envelope [from incoming messages].
  - inbox message [from envelopes].
  - dead letter message [from broken envelopes or inbox messages].
  - dead letter envelope [from dead letter message].
- inbound pipeline:
  - defines the cross-pipeline processing flow map.
- outbound pipeline segments:
  - outbox message [from developers].
  - envelope [from outbox messages].
- outbound pipeline:
  - defines the cross-pipeline processing flow map.

## Routers
- router:
  - decides next pipeline operation.
  - executes the next pipeline operation.
  - repeat process [decide/execute].
- inbound: orchestrates inbound pipelines operations.
- outbound: orchestrates outbound pipelines operations.

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

### Specialized Libraries
- foundation layer: kafka.* projects.

## Golden rules
- all projects on a layer may reference only projects from the layer immediately below.
- all projects within the same layer are independent of each other.
- all projects should use high cohesion internally, minimal coupling externally.
- broker-specific types and concepts must not leak into the core library.

## Packages
- meta-projects: only project references, no code [messaging.core, messaging.kafka].
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