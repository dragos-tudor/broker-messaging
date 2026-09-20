
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingDecision)>
    RunDeadLetteringPipelineAsync<
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
        DeadLetteringSignal,
        DeadLetteringDecision>(
          services,
          data,
          DeadLetteringEntry.Start,
          AdvanceDeadLetteringPipeline,
          ExecuteDeadLetteringOperationAsync<TServices, TData, TKey, TPayload>,
          PropagateDeadLetteringException<TData, TKey, TPayload>,
          CanFastRetryDeadLettering,
          ct);
}