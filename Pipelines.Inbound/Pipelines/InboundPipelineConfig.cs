
namespace Pipelines.Inbound;

public readonly ref struct InboundPipelineConfig
{
  public bool HandleAfterCapture { get; init; }
  public bool UseBrokerPublisher { get; init; }
}

