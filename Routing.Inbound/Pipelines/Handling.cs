
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingTransition)>
    RunHandlingPipelineAsync<
      TServices,
      TData,
      TKey,
      TValue,
      TMetadata,
      TConfirmation,
      TPayload,
      TSession>(
        TServices services,
        TData data,
        CancellationToken ct = default)
    where TServices : IInboundRunningServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRunningData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable =>
      RunInboundPipelineAsync<
        TServices,
        TData,
        TKey,
        TValue,
        TMetadata,
        TConfirmation,
        TPayload,
        TSession,
        HandlingSignal,
        HandlingTransition>(
          services,
          data,
          HandlingEntry.Start,
          GetHandlingTransition,
          ExecuteHandlingOperationAsync<TServices, TData, TKey, TPayload, TSession>,
          PropagateHandlingException<TData, TKey, TPayload>,
          CanFastRetryHandling,
          ct);
}