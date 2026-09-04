
## Coding Conventions [v1]

### Golden Rules
- build a project-specific vocabulary by mapping domain concepts to explicit named functions. Compose these functions at higher levels so feature code reads as a domain-specific language rather than implementation mechanics.
- bring behavior to the surface: prefer named functions over inline anonymous behavior.

### General Rules
- each feature-level function should have its own file.
- functions must be members of static classes; do not introduce instance classes to contain behavior.
- structure feature functions as readable top-to-bottom workflows; use early returns to avoid nesting.
- make data and behavior dependencies explicit through function parameters.

### File & Function Granularity
- prefer many small files containing single, focused functions [excepting tests].
- group related files within entity/feature folders.
- functions like getting or setting properties could be grouped into one file.
- use `partial` classes to keep functions grouped logically across files under a single static class.
- global usings:
  - centralize commonly used usings in a Using.cs per project.
  - centralize commonly used test usings in a Using.Tests.cs per project (for testing libraries ONLY).

### Coding Style
- use `record` for immutable data/options containers (e.g., KafkaOptions.cs).
- use `init` properties for immutable initialization.
- use `var` even when the right-hand type is not obvious.
- propagate `CancellationToken` in asynchronous APIs and default to `default`.
- be explicit about nullable types: `T?` for nullable returns/params.
- don't reassign parameters or locals; use new variables instead.

### Functional Coding Style
- separate data from behavior.
- write simple, pure, testable low-level functions (using an imperative style).
- combine/compose those simple functions into complex, testable, high-level functions that implement features (using declarative style).
- use higher-order functions (with callbacks/services for side effects).
- use early return to avoid `pyramid of doom`.
- prefer immutability over mutability.
- prefer functional collection operations/composition over explicit loops when appropriate.
- prefer named functions over inline lambda expressions:
  - prefer: `integers.Select(Increment)`.
  - avoid: `integers.Select(n => n + 1)`.
- inject dependencies explicitly through function parameters, service structures/interfaces, or delegates.
- use:
  - Result pattern for expected/domain failures.
  - reserve exceptions for technical runtime failures.

### Testing Conventions
- use:
  - `MSTest` as testing framework.
  - `Shouldly` for assertions.
  - `NSubstitute` for mocking.
- keep test files adjacent to the code they test (e.g., Verifying.Tests.cs next to Verifying.cs).

### Names
- use naming styles for methods:
  - `<Action><Object>` (e.g., `CaptureEnvelope`).
  - `<Action><Object><Property>` (e.g., `SetInboxMessageRetryCount`).
- use naming style for folders:
  - `<Objects>` (e.g., `InboxMessages`, `DeadLetterEnvelopes`).
  - `<Feature>` (e.g., `Handling`, `Scheduling`, `Publishing`) when necessary (based on examples).
- use descriptive names for parameters and variables.
- use human-readable test names with double underscores `subject__action__expected`.

