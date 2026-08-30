
namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static RetryMessage CreateRetryMessage(string retryId) =>
    new ()
    {
      RetryId = retryId,
      RetryCount = 0,
      CreatedAt = DateTime.UtcNow,
    };
}
