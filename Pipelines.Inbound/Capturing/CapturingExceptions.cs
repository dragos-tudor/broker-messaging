using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagateCapturingException<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
    CapturingSignal signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;
    var inboxMessage = ((Operations.Inbound.Inbox.IInboxMessageProp<TKey, TPayload>)data).InboxMessage;

    return signal switch
    {
      VerifyingStates.InvalidError
      or VerifyingStates.InvalidConfirmableError
      or VerifyingStates.Error
      or MappingStates.Error => data.Envelope?.FailureReason = exception.Message,
      ValidatingStates.InvalidError
      or ValidatingStates.Error => inboxMessage?.FailureReason = exception.Message,
      _ => default
    };
  }
}
