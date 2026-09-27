using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public static partial class OutboundFuncs
{
  internal const string BrokerMessageError = "Broker message was not acknowledged";

  internal static string? PropagateDispatchingException<TKey, TPayload>(object?[] data, string signal, Exception? exception)
  {
    if (exception is OperationCanceledException) return default;
    var message = GetOutboxMessage<TKey, TPayload>(data);

    return signal switch
    {
      DispatchingStates.NotAck => message?.LastError = exception?.Message ?? BrokerMessageError,
      DispatchingStates.Error => message?.LastError = exception?.Message,
      _ => default
    };
  }
}
