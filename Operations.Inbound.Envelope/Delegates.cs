namespace Operations.Inbound.Envelope;

public delegate IInboxMessage<TKey, TPayload>
  FromEnvelopeToInboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    DateTime currentDate,
    InboxMessageStatus status = InboxMessageStatus.Processing
  );

public delegate IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
  FromEnvelopeToDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    string failureReason,
    DateTime currentDate
  );
