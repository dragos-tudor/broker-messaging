using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateDeadLetteringException<TData, TKey, TPayload>(
    TData data,
    DeadLetteringSignal signal,
    Exception? exception)
  where TData: IDeadLetteringData<TKey, TPayload>
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
