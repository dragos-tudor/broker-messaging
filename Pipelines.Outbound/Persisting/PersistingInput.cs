using Outbox = Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

internal enum PersistingEntry { Start }

internal readonly union PersistingInput(
  PersistingEntry,
  Outbox.ValidatingStates,
  Outbox.TransactingStates
);
