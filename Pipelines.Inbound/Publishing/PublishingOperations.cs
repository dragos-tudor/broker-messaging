using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

readonly ref struct PublishingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.MappingStates, Exception?)>>? Mapping { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetterEnvelope.PublishingStates, Exception?)>>? Publishing { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetterEnvelope.ProducingStates, Exception?)>>? Producing { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.SchedulingStates, Exception?)>>? Scheduling { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.AbandoningStates, Exception?)>>? Abandoning { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.ClosingStates, Exception?)>>? Closing { get; init; }
}

partial class InboundFuncs
{
  internal static PublishingOperation<TServices, TData> GetPublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(PublishingActions action)
    where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => action switch
    {
      PublishingActions.Mapping => new PublishingOperation<TServices, TData> { Mapping = MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload> },
      PublishingActions.Publishing => new PublishingOperation<TServices, TData> { Publishing = PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload> },
      PublishingActions.Producing => new PublishingOperation<TServices, TData> { Producing = ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload> },
      PublishingActions.Scheduling => new PublishingOperation<TServices, TData> { Scheduling = ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      PublishingActions.Abandoning => new PublishingOperation<TServices, TData> { Abandoning = AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      PublishingActions.Closing => new PublishingOperation<TServices, TData> { Closing = CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      _ => throw new InvalidOperationException($"Unknown publishing action: {action}")
    };

  internal static ValueTask<(TData, PublishingInput, Exception?)> ExecutePublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(PublishingOperation<TServices, TData> operation, TServices services, TData data, CancellationToken ct = default)
    where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => operation switch
    {
      { Mapping: not null } => operation.Mapping(services, data, ct).FromResult<TData, DeadLetter.MappingStates, PublishingInput>(static state => state),
      { Publishing: not null } => operation.Publishing(services, data, ct).FromResult<TData, DeadLetterEnvelope.PublishingStates, PublishingInput>(static state => state),
      { Producing: not null } => operation.Producing(services, data, ct).FromResult<TData, DeadLetterEnvelope.ProducingStates, PublishingInput>(static state => state),
      { Scheduling: not null } => operation.Scheduling(services, data, ct).FromResult<TData, DeadLetter.SchedulingStates, PublishingInput>(static state => state),
      { Abandoning: not null } => operation.Abandoning(services, data, ct).FromResult<TData, DeadLetter.AbandoningStates, PublishingInput>(static state => state),
      { Closing: not null } => operation.Closing(services, data, ct).FromResult<TData, DeadLetter.ClosingStates, PublishingInput>(static state => state),
      _ => throw new InvalidOperationException("Unknown publishing operation.")
    };
}
