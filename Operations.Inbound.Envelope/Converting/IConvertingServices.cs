
namespace Operations.Inbound.Envelope;

public interface IConvertingServices<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation>,
  IEnvelopeQueueReaderService<TKey, TValue, TMetadata, TConfirmation>,
  IUtcDateService;

