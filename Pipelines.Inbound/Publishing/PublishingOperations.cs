using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, PublishingSignal, Exception?)> ExecutePublishingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(PublishingActions action, TServices services, TData data, CancellationToken ct = default)
    where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        PublishingActions.Mapping => MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, DeadLetter.MappingStates, PublishingSignal>(static state => state),
        PublishingActions.Publishing => PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, DeadLetterEnvelope.PublishingStates, PublishingSignal>(static state => state),
        PublishingActions.Producing => ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, DeadLetterEnvelope.ProducingStates, PublishingSignal>(static state => state),
        PublishingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.SchedulingStates, PublishingSignal>(static state => state),
        PublishingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.AbandoningStates, PublishingSignal>(static state => state),
        PublishingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.ClosingStates, PublishingSignal>(static state => state),
      };
}
