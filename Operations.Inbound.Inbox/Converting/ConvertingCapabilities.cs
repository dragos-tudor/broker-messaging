namespace Operations.Inbound.Inbox;

public sealed record ConvertingCapabilities (
  GetUtcDateTime GetUtcDateTime,
  FromInboxMessage FromInboxMessage
);
