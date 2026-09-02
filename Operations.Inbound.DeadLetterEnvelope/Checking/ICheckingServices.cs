
namespace Operations.Inbound.DeadLetterEnvelope;

public interface ICheckingRetryServices:
  ICheckingServices,
  IRetryPlanOptionsReaderService;
