
namespace Pipelines.Inbound;

public record InboundPipelineConfig
{
  public bool HandleAfterCapture { get; init; }
  public bool UseBrokerPublisher { get; init; }
}

