using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public static partial class OutboundFuncs
{
  internal static string? PropagatePublishingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    PublishingSignal signal,
    Exception? exception)
  where TData: IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      MappingStates.Error
      or PublishingStates.Error
      or ProducingStates.Error => data.OutboxMessage?.LastError = exception.Message,
      _ => default
    };
  }
}
