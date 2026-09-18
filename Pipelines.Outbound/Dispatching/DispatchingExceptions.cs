using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public static partial class OutboundFuncs
{
  internal static string? PropagateDispatchingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    DispatchingSignal signal,
    Exception? exception)
  where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      DispatchingStates.NotAck => data.OutboxMessage?.LastError = exception?.Message ?? "Broker message was not acknowledged",
      DispatchingStates.Error => data.OutboxMessage?.LastError = exception?.Message,
      _ => default
    };
  }
}
