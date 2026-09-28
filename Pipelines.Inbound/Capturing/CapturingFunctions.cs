
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static PipelineFunctions<CapturingCapabilities>
    CreateCapturingFunctions() =>
      new (
        AdvanceCapturingPipeline,
        ExecuteCapturingOperationAsync,
        PropagateCapturingException,
        CanFastRetryCapturing
      );
}