using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

internal readonly union HandlingSignal(
  HandlingEntries,
  Inbox.HandlingStates,
  Inbox.TransactingStates,
  Inbox.SchedulingStates,
  Inbox.AbandoningStates
);
