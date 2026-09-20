
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingDecision)>
    RunRedirectingPipelineAsync<
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
        RedirectingSignal,
        RedirectingDecision>(
          services,
          data,
          RedirectingEntry.Start,
          AdvanceRedirectingPipeline,
          ExecuteRedirectingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          static (data, signal, exception) => default,
          CanFastRetryRedirecting,
          ct);
}