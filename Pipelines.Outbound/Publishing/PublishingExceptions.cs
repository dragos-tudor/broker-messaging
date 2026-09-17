using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public static partial class OutboundFuncs
{
  internal static string? PropagatePublishingException<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
    PublishingSignal signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;

    var outboxMessage = ((Operations.Outbound.Outbox.IOutboxMessageProp<TKey, TPayload>)data).OutboxMessage;

    return signal switch
    {
      MappingStates.Error
      or PublishingStates.Error
      or ProducingStates.Error => outboxMessage?.LastError = exception.Message,
      _ => default
    };
  }
}
