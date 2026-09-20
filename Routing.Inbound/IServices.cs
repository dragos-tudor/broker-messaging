
namespace Routing.Inbound;

public interface IRoutingInboundInstrumentionServices
{
  void InstrumentPipeline<TSignal, TDecision>(TSignal signal, TDecision decision);
  void InstrumentOperation<TData, TState>(TData data, TState state, Exception? exception);
}