using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static ValueTask<(TData, PublishingSignal, Exception?)>
    ExecutePublishingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      PublishingTransition transition,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      transition switch
      {
        PublishingActions.Mapping => MapOutboxMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, MappingStates, PublishingSignal>(static state => state),
        PublishingActions.Producing => ProduceEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, ProducingStates, PublishingSignal>(static state => state),
        PublishingActions.Publishing => PublishEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, PublishingStates, PublishingSignal>(static state => state),
        PublishingActions.Scheduling => ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, SchedulingStates, PublishingSignal>(static state => state),
        PublishingActions.Abandoning => AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, PublishingSignal>(static state => state),
        PublishingActions.Closing => CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, PublishingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
      };
}
