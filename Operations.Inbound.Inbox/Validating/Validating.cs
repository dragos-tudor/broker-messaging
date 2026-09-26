using Funcs = Persistence.InboxMessage.InboxMessageFuncs;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (ValidatingData<TKey, TPayload>, string, Exception?)
    ValidateInboxMessageSuccess<TKey, TPayload>(
      ValidatingCapabilities capabilities,
      ValidatingData<TKey, TPayload> data)
    {
      var message = RequireInboxMessage(data.InboxMessage);

      var error = Funcs.ValidateInboxMessage(message);
      return error is not null?
        (data, ValidatingStates.InvalidError, Funcs.CreateValidationException(error)):
        (data, ValidatingStates.Success, null);
    }

  static (ValidatingData<TKey, TPayload>, string, Exception?)
    ValidateInboxMessageError<TKey, TPayload>(
      ValidatingData<TKey, TPayload> data,
      Exception exception) =>
    (data, ValidatingStates.Error, exception);

  internal static (ValidatingData<TKey, TPayload>, string, Exception?)
    ValidateInboxMessage<TKey, TPayload>(
      ValidatingCapabilities capabilities,
      ValidatingData<TKey, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      ValidateInboxMessageSuccess,
      ValidateInboxMessageError
    );
}
