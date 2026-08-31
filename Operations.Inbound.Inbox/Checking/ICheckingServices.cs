
namespace Operations.Inbound.Inbox;

public interface ICheckingRetryServices:
  ICheckingServices,
  IRetryMessageOptionsReaderService;