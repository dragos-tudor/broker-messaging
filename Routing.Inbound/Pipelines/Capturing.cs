
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingDecision)>
    RunCapturingPipelineAsync<
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
        CapturingSignal,
        CapturingDecision>(
          services,
          data,
          CapturingEntry.Start,
          AdvanceCapturingPipeline,
          ExecuteCapturingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          PropagateCapturingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          CanFastRetryCapturing,
          ct);
}