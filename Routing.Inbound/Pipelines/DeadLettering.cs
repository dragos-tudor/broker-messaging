
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
        DeadLetteringSignal,
        DeadLetteringDecision>(
          services,
          data,
          DeadLetteringEntries.Start,
          AdvanceDeadLetteringPipeline,
          ExecuteDeadLetteringOperationAsync<TServices, TData, TKey, TPayload>,
          PropagateDeadLetteringException<TData, TKey, TPayload>,
          CanFastRetryDeadLettering,
          ct);
}