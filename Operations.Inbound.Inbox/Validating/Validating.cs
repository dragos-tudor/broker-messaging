
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (object?[], string, Exception?)
    ValidateInboxMessageSuccess(
      ValidatingCapabilities capabilities,
      object?[] data)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));

      var error = capabilities.ValidateInboxMessage(message);
      return error is not null?
        (data, ValidatingStates.InvalidError, CreateValidationException(error)):
        (data, ValidatingStates.Success, null);
    }

  static (object?[], string, Exception?)
    ValidateInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ValidatingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ValidateInboxMessage(
      ValidatingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ValidateInboxMessageSuccess,
      ValidateInboxMessageError
    );
}
