
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static InboxMessage<TKey,TPayload> RequireInboxMessage<TKey,TPayload>(
    InboxMessage<TKey,TPayload>? message) =>
    message ?? throw new InvalidOperationException("Inbox message is required.");

  internal static TModel RequireInboxModel<TModel>(TModel? model) =>
    model ?? throw new InvalidOperationException("Inbox model is required.");
}