
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  static readonly PropagateException NoPropagationException = (_, __, ___) => string.Empty;

  internal static PipelineFunctions<PersistingCapabilities<TSession>>
    CreatePersistingFunctions<TSession>()
    where TSession : ISessionService =>
      new (
        AdvancePersistingPipeline,
        ExecutePersistingOperationAsync,
        NoPropagationException,
        CanFastRetryPersisting
      );
}