
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static PipelineFunctions<DispatchingCapabilities>
    CreateDispatchingFunctions() =>
      new (
        AdvanceDispatchingPipeline,
        ExecuteDispatchingOperationAsync,
        PropagateDispatchingException,
        CanFastRetryDispatching
      );
}