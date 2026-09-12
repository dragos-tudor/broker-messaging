
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Func<string, OutboundPipelineConfig, string?>? GetOutboundPipeline(string pipeline) =>
    pipeline switch
    {
      PipelinesTypes.Persisting => PersistingPipeline,
      PipelinesTypes.Publishing => PublishingPipeline,
      PipelinesTypes.Dispatching => DispatchingPipeline,
      _ => default
    };
}