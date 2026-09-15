using Inbox = Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

internal enum DeadLetteringEntry { Start }

internal readonly union DeadLetteringInput(
  DeadLetteringEntry,
  Inbox.ConvertingStates,
  DeadLetter.InsertingStates,
  Inbox.AbandoningStates,
  Inbox.ClosingStates
);
