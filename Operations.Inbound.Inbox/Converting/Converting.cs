
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (object?[], string, Exception?)
    ConvertInboxMessageSuccess(
      ConvertingCapabilities capabilities,
      object?[] data)
  {
    var message = RequireInboxMessage(GetInboxMessage(data));
    var deadLetter = capabilities.FromInboxMessage(message, capabilities.GetUtcDateTime());
    SetDeadLetterMessage(data, deadLetter);
    return (data, ConvertingStates.Success, null);
  }

  static (object?[], string, Exception?)
    ConvertInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ConvertingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ConvertInboxMessage(
      ConvertingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ConvertInboxMessageSuccess,
      ConvertInboxMessageError
    );
}
