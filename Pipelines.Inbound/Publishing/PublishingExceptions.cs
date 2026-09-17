using DeadLetter = Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagatePublishingException<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
    PublishingSignal signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;
    var message = ((DeadLetter.IDeadLetterMessageProp<TKey, TPayload>)data).DeadLetterMessage;

    return signal switch
    {
      DeadLetter.MappingStates.Error
      or PublishingStates.Error
      or ProducingStates.Error => message?.LastError = exception.Message,
      _ => default
    };
  }
}
