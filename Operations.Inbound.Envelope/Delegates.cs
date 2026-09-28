namespace Operations.Inbound.Envelope;

public delegate IInboxMessage
  FromEnvelopeToInboxMessage(
    IEnvelope envelope,
    DateTime currentDate,
    InboxMessageStatus status = InboxMessageStatus.Processing
  );

public delegate IDeadLetterEnvelope
  FromEnvelopeToDeadLetterEnvelope(
    IEnvelope envelope,
    string failureReason,
    DateTime currentDate
  );
