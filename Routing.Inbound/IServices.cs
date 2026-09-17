
namespace Routing.Inbound;

public interface IInstrumentiomServices
{
  void InstrumentPipeline<TSignal, TTransition>(TSignal signal, TTransition transition);
  void InstrumentOperation<TData, TState>(TData data, TState state, Exception? exception);
}