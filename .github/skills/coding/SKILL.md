---
name: coding
description: 'Coding instructions and guidelines for the project.'
---

# Coding instructions

## **Project Setup**
- **Target Framework:** net10.0.
- **Nullability:** `Nullable` enabled; prefer non-nullable references and annotate nullable types explicitly.
- **Implicit usings:** disabled; each project supplies a Using.cs for global usings.
- **Analyzers & Build:** `AnalysisMode=All`, `TreatWarningsAsErrors=true`, `RunAnalyzersDuringBuild=true`. Treat code-analysis warnings as errors; be conservative with `#pragma` usage.
- **Central package management:** `ManagePackageVersionsCentrally=true` using Directory.Packages.props.
- **Testing:** Tests live adjacent to feature code in `*.Tests.cs` files (e.g., Creating.Tests.cs next to Creating.cs).

## **Repository Structure**
- **Project-per-storage:** Persistence.\*, Transport.\*, Operations.\*, Pipelines.\*, Routung.\* ... Messaging.Core, Messaging.Kafka are METAPROJECTS; keep only references to other projects and avoid adding implementation code directly in these projects.
- **Functional folders:** Groups related logic within projects (e.g., `Persistence.*`, `Transport.*`, `Operations.*`, `Pipelines.*`, `Routung.*` ...).
- **Partial static modules:** Use `static partial class <Name>Funcs` (e.g., `DeadLetterFuncs`, `DeadLetterTests`) split across many files for modular functions.
- **Global usings:** Centralize commonly used usings in a Using.cs per project and commonly used test usings in a Using.Tests.cs per project.

## **Naming Conventions**
- **Types:** PascalCase (e.g., `InboxMessageRetryOptions`, `DeadLetterEnvelope`).
- **Static helper classes:** PascalCase + `Funcs` suffix (e.g., `InboundFuncs`, `EngineTests`).
- **Methods:** PascalCase; async methods suffixed with `Async` (e.g., `InsertInboxMessageAsync`).
- **Generic parameters:** `T...` style (e.g., `T1`, `T2`, `TResult`).
- **Parameters & locals:** camelCase (e.g., `source`, `value`, `cancellationToken`).

## **Names:**
- Use naming styles for methods:
  - `<Action><Object>` (e.g., `CaptureEnvelope`).
  - `<Action><Object><Property>` (e.g., `SetInboxMessageRetryCount`).
- Use naming style for folders:
  - `<Objects>` (e.g., `InboxMessages`, `DeadLetterEnvelopes`).
  - `<Feature>` (e.g., `Handling`, `Scheduling`, `Publishing`) when is necessary (based on examples).
- Avoid abbreviations; use full words (e.g., `DLMessages` instead of `KCons`).
- Avoid Hungarian notation or type prefixes (e.g., `strName`, `iCount`).
- Use descriptive names for parameters and variables.
- **Test names**: human-readable with double underscores `subject__action__expected` (e.g., `source__filter__results_returned`).
- **Fields/constants**: PascalCase; avoid leading underscores for private fields.


## **File & Function Granularity**
- Prefer many small files containing single, focused functions or small related groups within feature folders.
- Use `partial` classes to keep related helpers grouped logically across files under a single static class.
- Use for each project a `Using.cs` file for global usings and a `Using.Tests.cs` for test-wide usings.

## **Coding Style**
- Use functional programming paradigm.
- Use `record` for immutable data/options containers (e.g., KafkaOptions.cs).
- Use `init` properties for immutable initialization.
- Use `var` even when the right-hand type is not obvious.
- Use target-typed `new()` where readable.
- Keep functions small and single-purpose.
- Propagate `CancellationToken` in asynchronous APIs and default to `default`.
- Be explicit about nullable types: `T?` for nullable returns/params.
- Don't reassign parameters or locals; use new variables instead.
- Don't use expression-bodied members for trivial functions, extract into standalone functions.

## **Functional coding style**
- Separate data from behavior.
- Write simple, pure, testable low-level functions (using an imperative style).
- Combine/compose those simple functions into complex, testable, high-level functions that implement features (using declarative style).
- Use higher-order functions (such as callbacks for side effects).
- Bring all business logic and business structures to the surface.
- Prefer immutability over mutability.
- Prefer declarative over imperative.
- Use recursion over looping.
- Don't be afraid of Monads—you already use them in LINQ and Tasks anyway.
- Don't throw exceptions; use the Result pattern instead.

## **Design Patterns & Architectural Principles**
- **Functional composition via static funcs:** Logic lives in small pure-ish static functions grouped under `*Funcs` partial classes.
- **Dependency injection by delegates:** Pass behavior as services to decouple logic from infrastructure (e.g., `ITransactInboxMessageAsync`).
- **Adapter per persistence:** Avoid storage-specific implementations for different backends. Use callback to abstract storage operations.

## **Functional Programming Principles**
- Favor small pure functions that return data rather than mutate state.
- Compose behavior with higher-order functions (pass delegates).
- Localize side effects to specific storage adapters; keep logic side-effect-light.

## **Validation & Error Flows**
- Reserve exceptions for unexpected/unrecoverable runtime issues.
- Return results or allow callers to handle missing data via nullable returns or options.

## **Testing Conventions**
- Tests use `MSTest` and `Shouldly` for assertions.
- Use `CancellationToken.None` or `default` explicitly in test calls.
- Keep test files adjacent to the code they test (e.g., Verifying.Tests.cs next to Verifying.cs).

## **Formatting & Analyzer Usage**
<!-- - Keep code consistent with `AnalysisMode=All` and `PublicApiAnalyzers`.
- Treat analyzer results as primary style enforcement. -->
- All analyzers (including `PublicApiAnalyzers`) will be disabled for now. We will re-enable them once we have a stable baseline of code and tests.

## **Examples**
- Static partial class member:
  `partial class InboxFuncs { internal static async ValueTask<(TData, string, Exception?)> CloseInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>  ... }`
- public record:
  - `public record InboxMessageOptions { ... }`

## **Anti-patterns**
- Avoid large mutable services and stateful classes.
- Avoid hidden dependencies; prefer explicit function parameters or delegates.

## **How to add new features**
1. Identify the project (e.g., Pipelines.Inbound).
2. Add small partial functions under the appropriate `*Funcs` static partial class (e.g., `InboundFuncs`).
3. Place files in a focused folder describing the concern.
4. Add tests adjacent to the code in `*.Tests.cs` files.

## **References (repo hints)**
- Central configuration: Directory.Build.props.
- Package versions: Directory.Packages.props.