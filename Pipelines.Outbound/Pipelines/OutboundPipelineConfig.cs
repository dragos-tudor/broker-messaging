
namespace Pipelines.Outbound;

public readonly ref struct OutboundPipelineConfig
{
  internal bool UseBrokerPublisher { get; init; }
  internal bool PublishAfterPersist { get; init; }
}

