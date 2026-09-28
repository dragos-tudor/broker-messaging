
namespace Operations.Inbound.Inbox;

public delegate IDeadLetterMessage FromInboxMessage(IInboxMessage message, DateTime createdAt);