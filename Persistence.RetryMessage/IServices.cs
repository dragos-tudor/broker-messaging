
namespace Persistence.RetryMessage;

public interface ICheckingServices :
  IRetryMessageReaderService,
  IRetryMessageOptionsReaderService;

public interface IUpsertingServices :
  IRetryMessageUpsertService,
  IRetryMessageOptionsReaderService,
  IUtcDateService;

public interface IRetryMessageUpsertService
{
  Task UpsertRetryMessageAsync(
    RetryMessage message,
    Func<RetryMessage, RetryMessage> update,
    CancellationToken ct = default);
}

public interface IRetryMessageReaderService
{
  Task<RetryMessage?> GetRetryMessageByIdAsync(
    string retryId,
    CancellationToken ct = default);
}

public interface IRetryMessageOptionsReaderService
{
  RetryMessageOptions GetRetryMessageOptions();
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }



