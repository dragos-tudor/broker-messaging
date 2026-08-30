
namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static async ValueTask UpsertRetryMessageAsync<TServices, TKey>(
    TServices services,
    TKey key,
    DateTime createdAt,
    string error,
    CancellationToken ct = default)
  where TServices : IUpsertingServices
  {
    var retryId = BuildRetryMessageId(key, createdAt);
    var retryMessage = CreateRetryMessage(retryId);
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
