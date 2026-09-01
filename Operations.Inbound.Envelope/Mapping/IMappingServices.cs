
namespace Operations.Inbound.Envelope;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IEnvelopeValueMapperService<TValue, TPayload>,
  IUtcDateService;
