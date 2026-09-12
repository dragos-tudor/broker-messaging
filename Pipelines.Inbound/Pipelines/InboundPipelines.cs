
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<string, InboundPipelineConfig, string?>? GetInboundPipeline(string pipeline) =>
    pipeline switch
    {
      PipelineTypes.Capturing => CapturingPipeline,
      PipelineTypes.Redirecting => RedirectingPipeline,
      PipelineTypes.Handling => HandlingPipeline,
      PipelineTypes.DeadLettering => DeadLetteringPipeline,
      PipelineTypes.Publishing => PublishingPipeline,
      PipelineTypes.Dispatching => DispatchingPipeline,
      _ => default
    };
}