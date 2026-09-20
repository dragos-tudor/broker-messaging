
namespace Routing.Inbound;

public interface IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>:
  IInboundPipelineConfigService,
  IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>,
  IRoutingInboundInstrumentionServices,
  IFastRetryOptionsService
  where TSession: IDisposable;
