
namespace Kafka.Clients;

partial class ClientsTests
{
  static async Task<bool> WaitForAsync(
    Func<bool> func,
    bool expected,
    TimeSpan? retryAfter = default,
    CancellationToken cancellationToken = default)
  {
    var delay = retryAfter ?? TimeSpan.FromSeconds(0.5);
    while (!cancellationToken.IsCancellationRequested)
    {
      if (func() == expected) return expected;

      await Task.Delay(delay, cancellationToken);
    }
    return !expected;
  }

  static Task<bool> WaitForTrueAsync(
    Func<bool> func,
    TimeSpan? retryAfter = default,
    CancellationToken cancellationToken = default)
    => WaitForAsync(func, true, retryAfter, cancellationToken);

  static Task<bool> WaitForFalseAsync(
    Func<bool> func,
    TimeSpan? retryAfter = default,
    CancellationToken cancellationToken = default)
    => WaitForAsync(func, false, retryAfter, cancellationToken);
}