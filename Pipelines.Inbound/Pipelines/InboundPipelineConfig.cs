
namespace Pipelines.Inbound;

readonly ref struct InboundPipelineConfig
{
  internal bool HandleAfterCapture { get; init; }
  internal bool UseBrokerPublisher { get; init; }
}

