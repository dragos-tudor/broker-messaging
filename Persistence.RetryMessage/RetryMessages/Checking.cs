
namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static async ValueTask<bool> CheckRetryMessageExhaustedAsync<TServices, TKey>(
    TServices services,
    TKey key,
    DateTime createdAt,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  {
    var retryId = BuildRetryMessageId(key, createdAt);
    var retryMessage = await services.GetRetryMessageByIdAsync(retryId, ct);

    var options = services.GetRetryMessageOptions();
    return IsRetryMessageExhausted(retryMessage, options);
  }
}
