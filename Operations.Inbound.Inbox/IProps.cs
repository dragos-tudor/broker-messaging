
namespace Operations.Inbound.Inbox;

public interface IDeadLetterMessageProp<TKey, TPayload> { IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; } }

public interface IInboxMessageProp<TKey, TPayload> { IInboxMessage<TKey, TPayload>? InboxMessage { get; set; } }

public interface IDomainModelProp { object? DomainModel { get; set; } }


