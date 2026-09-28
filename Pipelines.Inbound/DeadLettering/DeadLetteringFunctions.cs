
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static PipelineFunctions<DeadLetteringCapabilities>
    CreateDeadLetteringFunctions() =>
      new (
        AdvanceDeadLetteringPipeline,
        ExecuteDeadLetteringOperationAsync,
        PropagateDeadLetteringException,
        CanFastRetryDeadLettering
      );
}