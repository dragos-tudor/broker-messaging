
namespace Pipelines.Inbound;

static class InboundPipelines
{
  internal const string Capturing = $"{nameof(Capturing)}";
  internal const string Redirecting = $"{nameof(Redirecting)}";
  internal const string Handling = $"{nameof(Handling)}";
  internal const string DeadLettering = $"{nameof(DeadLettering)}";
  internal const string Publishing = $"{nameof(Publishing)}";
  internal const string Dispatching = $"{nameof(Dispatching)}";
}

partial class InboundFuncs
{
  internal static Func<string, InboundPipelineConfig, string?>? GetInboundActionMapper(string pipeline) =>
    pipeline switch
    {
      InboundPipelines.Capturing => MapCapturingAction,
      InboundPipelines.Redirecting => MapRedirectingAction,
      InboundPipelines.Handling => MapHandlingAction,
      InboundPipelines.DeadLettering => MapDeadLetteringAction,
      InboundPipelines.Publishing => MapPublishingAction,
      InboundPipelines.Dispatching => MapDispatchingAction,
      _ => default
    };
}