
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingTransition)>
    RunDispatchingPipelineAsync<
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
        DispatchingSignal,
        DispatchingTransition>(
          services,
          data,
          DispatchingEntry.Start,
          GetDispatchingTransition,
          ExecuteDispatchingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          ct);
}