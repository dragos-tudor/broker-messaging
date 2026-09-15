using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

internal enum HandlingEntry { Start }

internal readonly union HandlingInput(
  HandlingEntry,
  Inbox.HandlingStates,
  Inbox.TransactingStates,
  Inbox.SchedulingStates,
  Inbox.AbandoningStates
);
