namespace Pipelines.Inbound;

public readonly record struct InboundPipelineConfig(bool PublishDeadLetterEnvelope = true);

