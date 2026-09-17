
namespace Routing.Inbound;

public interface IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>:
  IInboundRunningServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
  where TSession: IDisposable;
