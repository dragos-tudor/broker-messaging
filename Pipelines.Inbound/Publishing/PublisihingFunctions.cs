
namespace Pipelines.Inbound;

partial class InboundFuncs
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