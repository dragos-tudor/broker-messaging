
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingTransition)>
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
        PublishingSignal,
        PublishingTransition>(
          services,
          data,
          PublishingEntry.Start,
          AdvancePublishingPipeline,
          ExecutePublishingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          PropagatePublishingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
          CanFastRetryPublishing,
          ct);
}