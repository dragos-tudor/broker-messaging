namespace Reliability.Resiliency;

partial class ResiliencyTests
{
  [TestMethod]
  public async Task verify_fast_retry__retry_count_below_maximum__allows_retry()
  {
    var options = new FastRetryOptions
    {
      MaxRetryAttempts = 1,
      RetryBaseDelay = TimeSpan.Zero
    };

    var result = await IsFastRetryDelayedAsync(0, options);

    result.ShouldBeTrue();
  }

  [TestMethod]
  public async Task verify_fast_retry__retry_count_at_maximum__declines_retry()
  {
    var options = new FastRetryOptions
    {
      MaxRetryAttempts = 1,
      RetryBaseDelay = TimeSpan.Zero
    };

    var result = await IsFastRetryDelayedAsync(1, options);

    result.ShouldBeFalse();
  }

  [TestMethod]
  public async Task verify_fast_retry__zero_maximum_attempts__declines_first_retry()
  {
    var options = new FastRetryOptions
    {
      MaxRetryAttempts = 0,
      RetryBaseDelay = TimeSpan.Zero
    };

    var result = await IsFastRetryDelayedAsync(0, options);

    result.ShouldBeFalse();
  }

  [TestMethod]
  public async Task verify_fast_retry__cancelled_token__declines_retry()
  {
    using var cancellationSource = new CancellationTokenSource();
    await cancellationSource.CancelAsync();

    var result = await IsFastRetryDelayedAsync(0, new FastRetryOptions(), cancellationSource.Token);

    result.ShouldBeFalse();
  }
}
