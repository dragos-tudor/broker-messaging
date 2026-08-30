
namespace Operations.Inbound.Envelope;

public interface IConvertingServices<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation>,
  IEnvelopeQueueReaderService<TKey, TValue, TMetadata, TConfirmation>,
  IUtcDateService;

public interface IDeadLetterEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation> {
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> FromEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    string queueName,
    string failureReason,
    DateTime currentDate);
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }