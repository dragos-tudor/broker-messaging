using Funcs = Persistence.InboxMessage.InboxMessageFuncs;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (object?[], string, Exception?)
    ValidateInboxMessageSuccess<TKey, TPayload>(
      ValidatingCapabilities<TKey, TPayload> capabilities,
      object?[] data)
    {
      var message = RequireInboxMessage(GetInboxMessage<TKey, TPayload>(data));

      var error = Funcs.ValidateInboxMessage(message);
      return error is not null?
        (data, ValidatingStates.InvalidError, Funcs.CreateValidationException(error)):
        (data, ValidatingStates.Success, null);
    }

  static (object?[], string, Exception?)
    ValidateInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ValidatingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ValidateInboxMessage<TKey, TPayload>(
      ValidatingCapabilities<TKey, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ValidateInboxMessageSuccess,
      ValidateInboxMessageError
    );
}
