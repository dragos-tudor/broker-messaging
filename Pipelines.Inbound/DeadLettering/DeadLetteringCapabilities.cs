using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public sealed record DeadLetteringCapabilities
(
  ConvertingCapabilities Converting,
  Operations.Inbound.DeadLetter.InsertingCapabilities Inserting,
  AbandoningCapabilities Abandoning,
  ClosingCapabilities Closing
);
