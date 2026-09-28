namespace Operations.Outbound.Outbox;

public delegate IEnvelope FromOutboxMessage(IOutboxMessage message, DateTime currentDate);
