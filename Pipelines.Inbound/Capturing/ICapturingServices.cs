using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface ICapturingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICapturingServices<TKey, TValue, TMetadata, TConfirmation>,
  IVerifyingServices<TKey, TValue, TMetadata, TConfirmation>,
  IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IValidatingServices,
  IInsertingServices<TKey, TPayload>,
  IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>;