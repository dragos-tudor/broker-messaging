
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static PipelineFunctions<PublishingCapabilities>
    CreatePublishingFunctions() =>
      new (
        AdvancePublishingPipeline,
        ExecutePublishingOperationAsync,
        PropagatePublishingException,
        CanFastRetryPublishing
      );
}