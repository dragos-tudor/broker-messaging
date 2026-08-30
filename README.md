## Broker agnostic client library.
Implement transactional inbox/outbox pattern as broker agnostic
(*ongoing desing/architecture docs on [docs](./docs)*).

### Remarks
---
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