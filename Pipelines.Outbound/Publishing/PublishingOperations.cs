using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

readonly ref struct PublishingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, MappingStates, Exception?)>>? Mapping { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, PublishingStates, Exception?)>>? Publishing { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ProducingStates, Exception?)>>? Producing { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, SchedulingStates, Exception?)>>? Scheduling { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, AbandoningStates, Exception?)>>? Abandoning { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ClosingStates, Exception?)>>? Closing { get; init; }
}

partial class OutboundFuncs
{
  internal static PublishingOperation<TServices, TData>
    GetPublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(PublishingActions action)
      where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirming, TPayload>
      where TData : IPublishingData<TKey, TValue, TMetadata, TConfirming, TPayload> =>
      action switch
      {
        PublishingActions.Mapping => new PublishingOperation<TServices, TData> { Mapping = MapOutboxMessage<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload> },
        PublishingActions.Producing => new PublishingOperation<TServices, TData> { Producing = ProduceEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload> },
        PublishingActions.Publishing => new PublishingOperation<TServices, TData> { Publishing = PublishEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload> },
        PublishingActions.Scheduling => new PublishingOperation<TServices, TData> { Scheduling = ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload> },
        PublishingActions.Abandoning => new PublishingOperation<TServices, TData> { Abandoning = AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload> },
        PublishingActions.Closing => new PublishingOperation<TServices, TData> { Closing = CloseOutboxMessageAsync<TServices, TData, TKey, TPayload> },
        _ => throw new InvalidOperationException($"Unknown publishing action: {action}")
      };

  internal static ValueTask<(TData, PublishingInput, Exception?)> ExecutePublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(PublishingOperation<TServices, TData> operation, TServices services, TData data, CancellationToken ct = default)
    where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => operation switch
    {
      { Mapping: not null } => operation.Mapping(services, data, ct).FromResult<TData, MappingStates, PublishingInput>(static state => state),
      { Publishing: not null } => operation.Publishing(services, data, ct).FromResult<TData, PublishingStates, PublishingInput>(static state => state),
      { Producing: not null } => operation.Producing(services, data, ct).FromResult<TData, ProducingStates, PublishingInput>(static state => state),
      { Scheduling: not null } => operation.Scheduling(services, data, ct).FromResult<TData, SchedulingStates, PublishingInput>(static state => state),
      { Abandoning: not null } => operation.Abandoning(services, data, ct).FromResult<TData, AbandoningStates, PublishingInput>(static state => state),
      { Closing: not null } => operation.Closing(services, data, ct).FromResult<TData, ClosingStates, PublishingInput>(static state => state),
      _ => throw new InvalidOperationException("Unknown publishing operation.")
    };
}
