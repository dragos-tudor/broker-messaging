namespace Pipelines.Outbound;

public record OutboundPipelineConfig
{
  public bool UseBrokerPublisher { get; init; }
  public bool PublishAfterPersist { get; init; }
}
