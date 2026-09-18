using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateDispatchingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    DispatchingSignal signal,
    Exception? exception)
  where TData: IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      DispatchingStates.NotAck => data.DeadLetterMessage?.LastError = exception?.Message ?? "Broker message was not acknowledged",
      DispatchingStates.Error => data.DeadLetterMessage?.LastError = exception?.Message,
      _ => default
    };
  }
}
