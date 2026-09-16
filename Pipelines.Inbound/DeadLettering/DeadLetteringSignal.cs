using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

internal enum DeadLetteringEntry { Start }

internal readonly union DeadLetteringSignal(
  DeadLetteringEntry,
  ConvertingStates,
  DeadLetter.InsertingStates,
  AbandoningStates,
  ClosingStates
);
