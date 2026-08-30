
namespace Operations.Inbound.Inbox;

public interface IConvertingServices :
  IUtcDateService;

 public interface IUtcDateService { DateTime GetUtcDateTime(); }
