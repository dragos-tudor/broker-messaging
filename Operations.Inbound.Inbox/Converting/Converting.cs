
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ConvertInboxMessage<TServices, TData, TKey, TPayload>(
      TServices services,
      TData data,
      CancellationToken ct = default)
      where TServices : IConvertingServices
      where TData : IConvertingData<TKey, TPayload>
  {
    try
    {
      var inboxMessage = RequireInboxMessage(data.InboxMessage);
      var error = inboxMessage.LastError ?? "Unknown converting inbox message error.";

      var deadLetterMessage = FromInboxMessage(inboxMessage, error, services.GetUtcDateTime());
      data.DeadLetterMessage = deadLetterMessage;

      return new((data, ConvertInboxMessageSuccessState, null));
    }
    catch (Exception exception)
    {
      return new((data, ConvertInboxMessageErrorState, exception));
    }
  }
}