
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingTransition)>
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
        RedirectingSignal,
        RedirectingTransition>(
          services,
          data,
          RedirectingEntry.Start,
          GetRedirectingTransition,
          ExecuteRedirectingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          static (data, signal, exception) => default,
          ct);
}