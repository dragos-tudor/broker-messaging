using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
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
      or ProducingStates.Error => data.DeadLetterMessage?.LastError = exception.Message,
      _ => default
    };
  }
}
