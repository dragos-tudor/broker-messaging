
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (ConvertingData<TKey, TPayload>, string, Exception?)
    ConvertInboxMessageSuccess<TKey, TPayload>(
      ConvertingCapabilities capabilities,
      ConvertingData<TKey, TPayload> data)
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var deadLetter = FromInboxMessage(message, capabilities.GetUtcDateTime());
    return (data with { DeadLetterMessage = deadLetter }, ConvertingStates.Success, null);
  }

  static (ConvertingData<TKey, TPayload>, string, Exception?)
    ConvertInboxMessageError<TKey, TPayload>(
      ConvertingData<TKey, TPayload> data,
      Exception exception) =>
    (data, ConvertingStates.Error, exception);

  internal static (ConvertingData<TKey, TPayload>, string, Exception?)
    ConvertInboxMessage<TKey, TPayload>(
      ConvertingCapabilities capabilities,
      ConvertingData<TKey, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      ConvertInboxMessageSuccess,
      ConvertInboxMessageError
    );
}
