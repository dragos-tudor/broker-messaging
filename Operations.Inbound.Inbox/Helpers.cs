
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static object RequireDomainModel(object? model) =>
    model ?? throw new InvalidOperationException("Inbound domain model is required.");

  static IInboxMessage<TKey,TPayload> RequireInboxMessage<TKey,TPayload>(
    IInboxMessage<TKey,TPayload>? message) =>
    message ?? throw new InvalidOperationException("Inbox message is required.");
}

partial class InboxFuncs
{
  static IDeadLetterMessage<TKey, TPayload> SetDeadLetterMessage<TKey, TPayload>(
    IDeadLetterMessageProp<TKey, TPayload> data,
    IDeadLetterMessage<TKey, TPayload> message) =>
      data.DeadLetterMessage = message;

  static object SetDomainModel(IDomainModelProp data, object domainModel) =>
    data.DomainModel = domainModel;
}