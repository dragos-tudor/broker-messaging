
namespace Operations.Inbound.Inbox;

partial class InboxTests
{
  static object?[] CreateInboxData(
    IInboxMessage? message = null,
    IDeadLetterMessage? deadLetterMessage = null,
    object? model = null)
  {
    object?[] data = new object?[6];
    if (message is not null)
      SetInboxMessage(data, message);
    if (deadLetterMessage is not null)
      SetDeadLetterMessage(data, deadLetterMessage);
    if (model is not null)
      InboxFuncs.SetDomainModel(data, model);
    return data;
  }
}