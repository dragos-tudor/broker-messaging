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
- runners.

## Foundation Core Structures
- transport:
  - envelope broker-agnostic.
  - dead letter envelope broker-agnostic.
- persistence:
  - inbox message.
  - dead letter message.
  - outbox message.

## Operations
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
  - capturing incoming messages.
  - redirecting broken envelopes.
  - handling inbox messages.
  - dead-lettering inbox messages.
  - publishing dead letter messages.
  - dispatching enqueued dead letter messages.
- outbound pipeline segments:
  - persisting [from developers].
  - publishing [from outbox messages].

## Routers
- router: orchestrates pipeline operations by:
  - identifying/using current pipeline.
  - getting the pipeline operation.
  - executing the operation.
  - processing the operation result.
  - repeating the process [identify/get/execute/process].
- routers:
  - inbound.
  - outbound.

## Runners
- runner: drives message processing by invoking routers.
- inbound runners:
  - consuming runner: runs the inbound router continuously.
  - handling job: runs the inbound router for failed inbox messages.
  - dead lettering job: runs the inbound router for abandoned inbox messages.
  - publishing job: runs the inbound router for failed dead letter messages.
  - dispatching channels: runs the inbound router for producer ack results.
- outbound runners:
  - publishing job: runs the outbound router for failed outbox messages.
  - dispatching channels: runs the outbound router for producer ack results.

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
- foundation layer: persistence.*\, transport.\*, foundation.\* projects.
- operations layer: operations.* projects.
- pipelines layer: pipelines.* project.
- routing layer: routing.* projects.
- runners layer: runner project(s).
- system layer: reliability.* and observability.* projects.

### Specialized Libraries
- foundation layer: eg. kafka.* projects.

## Golden rules
- all projects on a layer may reference only projects from the layer immediately below.
- all projects within the same layer are independent of each other.
- all projects should use high cohesion internally, minimal coupling externally.
- broker-specific types and concepts must not leak into the core library.

## Packages
- meta-projects refer top-level projects and contain no implementation.
- meta-projects are packed as NuGet package.
- meta-projects are composition/packaging boundaries.
- messaging.core package packs all core project assemblies.
- messaging.kafka package packs all kafka project assemblies.

## Main Patterns
- transactional inbox pattern for inbound messages.
- transactional outbox pattern for outbound messages.
- at-least-once strategy for inbound and outbound messages.
- idempotency for inbound messages.
- composition root for services and data interfaces.
- orchestrator pattern for pipelines.
- fractal architectural pattern:
  - independent modules form the foundation at each level of composition.
  - higher layers compose them into progressively more capable modules.
  - the same principle applies recursively from functions to projects and libraries.