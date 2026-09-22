
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingDecision)>
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
    where TServices : IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRoutingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
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
        HandlingDecision>(
          services,
          data,
          HandlingEntries.Start,
          AdvanceHandlingPipeline,
          ExecuteHandlingOperationAsync<TServices, TData, TKey, TPayload, TSession>,
          PropagateHandlingException<TData, TKey, TPayload>,
          CanFastRetryHandling,
          ct);
}