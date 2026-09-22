
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingDecision)>
    RunPublishingPipelineAsync<
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
        PublishingSignal,
        PublishingDecision>(
          services,
          data,
          PublishingEntries.Start,
          AdvancePublishingPipeline,
          ExecutePublishingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          PropagatePublishingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          CanFastRetryPublishing,
          ct);
}