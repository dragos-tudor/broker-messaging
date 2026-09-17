using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateDispatchingException<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
    DispatchingSignal signal,
    Exception? exception)
  {
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      DispatchingStates.NotAck => data.DeadLetterMessage?.LastError = exception?.Message ?? "Broker message was not acknowledged",
      DispatchingStates.Error when exception is not null => data.DeadLetterMessage?.LastError = exception.Message,
      _ => default
    };
  }
}
