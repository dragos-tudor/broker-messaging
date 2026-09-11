using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDispatchingData,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
