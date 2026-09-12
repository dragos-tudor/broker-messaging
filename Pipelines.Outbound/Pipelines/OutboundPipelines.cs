
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Func<string, OutboundPipelineConfig, string?>? GetOutboundPipeline(string pipeline) =>
    pipeline switch
    {
      PipelinesTypes.Persisting => GetPersistingAction,
      PipelinesTypes.Publishing => GetPublishingAction,
      PipelinesTypes.Dispatching => GetDispatchingAction,
      _ => default
    };
}