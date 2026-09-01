
namespace Pipelines.Inbound;

readonly ref struct InboundMappingConfig
{
  internal bool UseBrokerPublisher { get; init; }
  internal bool ShouldHandleMessage { get; init; }
}

