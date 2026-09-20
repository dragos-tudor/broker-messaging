
namespace Routing.Inbound;

public interface IInboundRoutingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IInboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>;