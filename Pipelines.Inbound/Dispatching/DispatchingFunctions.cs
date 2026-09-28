
namespace Pipelines.Inbound;

partial class InboundFuncs
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