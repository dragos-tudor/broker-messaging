
namespace Pipelines.Inbound;

public record PipelineConfig
{
  public bool HandleAfterCapture { get; init; }
  public bool UseBrokerPublisher { get; init; }
}

