using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateDeadLetteringException(
    object?[] data,
    string signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;
    var message = GetInboxMessage(data);

    return signal switch
    {
      ConvertingStates.Error => message?.LastError = exception.Message,
      _ => default
    };
  }
}
