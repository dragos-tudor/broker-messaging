using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

internal enum HandlingEntry { Start }

internal readonly union HandlingSignal(
  HandlingEntry,
  Inbox.HandlingStates,
  Inbox.TransactingStates,
  Inbox.SchedulingStates,
  Inbox.AbandoningStates
);
