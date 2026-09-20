
namespace Routing.Inbound;

public interface IInboundRunningData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IInboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>;