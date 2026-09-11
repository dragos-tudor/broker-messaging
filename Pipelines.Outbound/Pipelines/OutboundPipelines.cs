#pragma warning disable CS8509

namespace Pipelines.Outbound;

static class OutboundPipelines
{
  internal const string Persisting = $"{nameof(Persisting)}";
  internal const string Publishing = $"{nameof(Publishing)}";
  internal const string Dispatching = $"{nameof(Dispatching)}";
}

partial class OutboundFuncs
{
  static Func<string, OutboundPipelineConfig, string?>? GetOutboundPipeline(string pipeline) =>
    pipeline switch
    {
      OutboundPipelines.Persisting => GetPersistingAction,
      OutboundPipelines.Publishing => GetPublishingAction,
      OutboundPipelines.Dispatching => GetDispatchingAction,
      _ => default
    };
}