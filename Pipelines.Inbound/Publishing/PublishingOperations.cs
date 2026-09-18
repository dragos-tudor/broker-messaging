using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, PublishingSignal, Exception?)>
    ExecutePublishingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      PublishingTransition transition,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      transition switch
      {
        PublishingActions.Mapping => MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data).FromResult<TData, MappingStates, PublishingSignal>(static state => state),
        PublishingActions.Publishing => PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, PublishingStates, PublishingSignal>(static state => state),
        PublishingActions.Producing => ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data).FromResult<TData, ProducingStates, PublishingSignal>(static state => state),
        PublishingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, SchedulingStates, PublishingSignal>(static state => state),
        PublishingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, PublishingSignal>(static state => state),
        PublishingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, PublishingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
      };
}
