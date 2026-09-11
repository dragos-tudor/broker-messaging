using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public interface IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDispatchingData,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
