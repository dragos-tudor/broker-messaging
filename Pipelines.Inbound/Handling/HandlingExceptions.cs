using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateHandlingException(
    object?[] data,
    string signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;
    var message = GetInboxMessage(data);

    return signal switch
    {
      HandlingStates.DomainError => message?.FailureReason = exception.Message,
      HandlingStates.Error or TransactingStates.Error => message?.LastError = exception.Message,
      _ => default
    };
  }
}
