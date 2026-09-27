
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (object?[], string, Exception?)
    ConvertInboxMessageSuccess<TKey, TPayload>(
      ConvertingCapabilities<TKey, TPayload> capabilities,
      object?[] data)
  {
    var message = RequireInboxMessage(GetInboxMessage<TKey, TPayload>(data));
    var deadLetter = FromInboxMessage(message, capabilities.GetUtcDateTime());
    SetDeadLetterMessage(data, deadLetter);
    return (data, ConvertingStates.Success, null);
  }

  static (object?[], string, Exception?)
    ConvertInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ConvertingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ConvertInboxMessage<TKey, TPayload>(
      ConvertingCapabilities<TKey, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ConvertInboxMessageSuccess,
      ConvertInboxMessageError
    );
}
