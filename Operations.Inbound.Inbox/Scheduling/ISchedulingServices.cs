
namespace Operations.Inbound.Inbox;

public interface ISchedulingServices<TKey, TPayload>:
  IInboxMessageUpdateService<TKey, TPayload>,
  IInboxRetryOptionsReaderService,
  IUtcDateService;