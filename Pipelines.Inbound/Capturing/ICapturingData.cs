using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICapturingData<TKey, TValue, TMetadata, TConfirmation>,
  IVerifyingData<TKey, TValue, TMetadata, TConfirmation>,
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IValidatingData<TKey, TPayload>,
  IInsertingData<TKey, TPayload>,
  IConfirmingData<TKey, TValue, TMetadata, TConfirmation>;