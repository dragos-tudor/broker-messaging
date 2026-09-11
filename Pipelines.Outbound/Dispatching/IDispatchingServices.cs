using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public interface IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDispatchingServices,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;

