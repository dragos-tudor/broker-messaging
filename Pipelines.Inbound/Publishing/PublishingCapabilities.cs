using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public sealed record PublishingCapabilities
(
  MappingCapabilities Mapping,
  ProducingCapabilities Producing,
  Operations.Inbound.DeadLetterEnvelope.PublishingCapabilities Publishing,
  SchedulingCapabilities Scheduling,
  AbandoningCapabilities Abandoning,
  ClosingCapabilities Closing
);
