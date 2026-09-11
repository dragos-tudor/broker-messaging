using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDispatchingServices,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;

