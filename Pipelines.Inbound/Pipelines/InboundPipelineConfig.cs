
namespace Pipelines.Inbound;

public readonly ref struct InboundPipelineConfig
{
  internal bool HandleAfterCapture { get; init; }
  internal bool UseBrokerPublisher { get; init; }
}

