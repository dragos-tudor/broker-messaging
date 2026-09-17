using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateDeadLetteringException<TKey, TPayload>(
    IDeadLetteringData<TKey, TPayload> data,
    DeadLetteringSignal signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      ConvertingStates.Error => data.InboxMessage?.LastError = exception.Message,
      _ => default
    };
  }
}
