
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static PipelineFunctions<HandlingCapabilities<TSession>>
    CreateHandlingFunctions<TSession>()
    where TSession : ISessionService =>
      new (
        AdvanceHandlingPipeline,
        ExecuteHandlingOperationAsync,
        PropagateHandlingException,
        CanFastRetryHandling
      );
}