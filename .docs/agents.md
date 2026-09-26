## Default Context

For all conversations use as context, besides the codebase, the documents from the .docs folder.

## Change Safety

- Change only what the task requires. Preserve unrelated behavior, structure, formatting, and conventions.
- Treat existing code, tests, and representative implementations as evidence of repository intent. Prefer them over generic conventions or personal implementation choices.
- Never weaken the verification mechanism to make an implementation pass. Do not remove, disable, skip, relax, or rewrite tests merely to accommodate an implementation.
- Follow established testing patterns and tooling from nearby representative tests unless the target requires something different.
- Do not introduce stylistic cleanup, code compression, unrelated refactoring, or alternative abstractions unless explicitly requested.
- Do not assume architectural or semantic intent when it cannot be established from the repository or provided context. Ask for clarification instead.

## Scaling Changes

- For structural migrations across sibling projects, complete and validate one project before modifying the next.
- Use an approved project as a reference for the transformation, not as a template to copy mechanically.
- Inspect each target independently and preserve its legitimate differences in contracts, data flow, nullability, operations, dependencies, and tests.
- Do not propagate an unresolved assumption from one sibling project to another.