using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateHandlingException<TData, TKey, TPayload>(
    TData data,
    HandlingSignal signal,
    Exception? exception)
  where TData : IHandlingData<TKey, TPayload>
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      HandlingStates.DomainError => data.InboxMessage?.FailureReason = exception.Message,
      HandlingStates.Error
      or TransactingStates.Error => data.InboxMessage?.LastError = exception.Message,
      _ => default
    };
  }
}
