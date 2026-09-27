namespace Operations.Inbound.Inbox;

public sealed record ConvertingCapabilities<TKey, TPayload> (
  GetUtcDateTime GetUtcDateTime
);
