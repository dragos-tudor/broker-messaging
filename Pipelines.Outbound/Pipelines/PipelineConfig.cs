namespace Pipelines.Outbound;

public record PipelineConfig
{
  public bool UseBrokerPublisher { get; init; }
  public bool PublishAfterPersist { get; init; }
}
