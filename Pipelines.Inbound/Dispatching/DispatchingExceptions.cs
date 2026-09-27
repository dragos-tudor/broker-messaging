using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal const string BrokerMessageException = "Broker message was not acknowledged";

  internal static string? PropagateDispatchingException<TKey, TPayload>(object?[] data, string signal, Exception? exception)
  {
    if (exception is OperationCanceledException) return default;
    var message = GetDeadLetterMessage<TKey, TPayload>(data);

    return signal switch
    {
      DispatchingStates.NotAck => message?.LastError = exception?.Message ?? BrokerMessageException,
      DispatchingStates.Error => message?.LastError = exception?.Message,
      _ => default
    };
  }
}
