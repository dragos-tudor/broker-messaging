namespace Operations.Inbound.DeadLetter;

public delegate IDeadLetterEnvelope
  FromDeadLetterMessage(
    IDeadLetterMessage message,
    DateTime currentDate
  );
