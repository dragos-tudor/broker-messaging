
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<string, InboundPipelineConfig, string?>? GetInboundPipeline(string pipelineType) =>
    pipelineType switch
    {
      PipelineTypes.Capturing => GetCapturingAction,
      PipelineTypes.Redirecting => GetRedirectingAction,
      PipelineTypes.Handling => GetHandlingAction,
      PipelineTypes.DeadLettering => GetDeadLetteringAction,
      PipelineTypes.Publishing => GetPublishingAction,
      PipelineTypes.Dispatching => GetDispatchingAction,
      _ => default
    };
}