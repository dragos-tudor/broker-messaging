using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

internal readonly union DeadLetteringSignal(
  DeadLetteringEntries,
  ConvertingStates,
  DeadLetter.InsertingStates,
  AbandoningStates,
  ClosingStates
);
