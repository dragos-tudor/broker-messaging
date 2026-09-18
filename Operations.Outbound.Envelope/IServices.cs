
namespace Operations.Outbound.Envelope;

public interface IProduceResultDispatcherService {
  void DispatchProduceResult(ProduceResult result);
}