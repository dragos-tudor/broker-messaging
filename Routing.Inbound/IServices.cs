
namespace Routing.Inbound;

public interface IRoutingInboundInstrumentionServices
{
  void InstrumentPipeline<TServices, TSignal, TDecision>(TServices services, TSignal signal, TDecision decision)
    where TSignal: struct
    where TDecision: struct;
  void InstrumentOperation<TServices, TData, TSignal>(TServices services, TData data, TSignal signal, Exception? exception)
    where TSignal: struct;
}