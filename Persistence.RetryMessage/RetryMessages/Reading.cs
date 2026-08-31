
namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static async Task<RetryMessage?> GetRetryMessageAsync<TServices, TKey>(
    TServices services,
    TKey key,
    DateTime createdAt,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  {
    var retryId = BuildRetryMessageId(key, createdAt);
    var retryMessage = await services.GetRetryMessageByIdAsync(retryId, ct);
    return retryMessage;
  }
}
