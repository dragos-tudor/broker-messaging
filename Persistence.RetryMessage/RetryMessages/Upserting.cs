
namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static async Task UpsertRetryMessageAsync<TServices>(
    TServices services,
    RetryMessage retryMessage,
    string error,
    CancellationToken ct = default)
  where TServices : IUpsertingServices
  {
    var options = services.GetRetryMessageOptions();
    var nextRetryCount = retryMessage.RetryCount + 1;
    var nextAttemptDate = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), options);

    await services.UpsertRetryMessageAsync(
      retryMessage,
      retryMessage =>
        SetRetryMessageRetryCount(retryMessage, nextRetryCount).
        SetRetryMessageNextAttemptAt(nextAttemptDate).
        SetRetryMessageLastError(error),
      ct);
  }
}
