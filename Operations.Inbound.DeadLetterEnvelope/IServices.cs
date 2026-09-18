
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IProduceResultDispatcherService {
  void DispatchProduceResult(ProduceResult result);
}