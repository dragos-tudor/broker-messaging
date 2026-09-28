using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateCapturingException(
    object?[] data,
    string signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;

    var envelope = GetEnvelope(data);
    var message = GetInboxMessage(data);

    return signal switch
    {
      VerifyingStates.InvalidError
      or VerifyingStates.InvalidConfirmableError
      or VerifyingStates.Error
      or MappingStates.Error => envelope?.FailureReason = exception.Message,
      ValidatingStates.InvalidError
      or ValidatingStates.Error => message?.FailureReason = exception.Message,
      _ => default
    };
  }
}
