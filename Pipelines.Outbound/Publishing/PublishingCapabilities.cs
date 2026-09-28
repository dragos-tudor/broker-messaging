using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public sealed record PublishingCapabilities(
  MappingCapabilities Mapping,
  ProducingCapabilities Producing,
  Operations.Outbound.Envelope.PublishingCapabilities Publishing,
  SchedulingCapabilities Scheduling,
  AbandoningCapabilities Abandoning,
  ClosingCapabilities Closing
);
