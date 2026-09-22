using Outbox = Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

internal readonly union PersistingSignal(
  PersistingEntries,
  Outbox.ValidatingStates,
  Outbox.TransactingStates
);
