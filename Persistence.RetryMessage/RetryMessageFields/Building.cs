
namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static string BuildRetryMessageId<TKey>(TKey key, DateTime createdAt) =>
    $"{key}:{createdAt:O}";
}
