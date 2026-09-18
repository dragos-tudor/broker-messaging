using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateCapturingException<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    CapturingSignal signal,
    Exception? exception)
  where TData: ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;

    return signal switch
    {
      VerifyingStates.InvalidError
      or VerifyingStates.InvalidConfirmableError
      or VerifyingStates.Error
      or MappingStates.Error => data.Envelope?.FailureReason = exception.Message,
      ValidatingStates.InvalidError
      or ValidatingStates.Error => data.InboxMessage?.FailureReason = exception.Message,
      _ => default
    };
  }
}
