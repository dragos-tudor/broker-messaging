namespace Reliability.Resiliency;

partial class ResiliencyTests
{
  [TestMethod]
  public async Task delay_fast_retry__zero_delay__completes()
  {
    var result = await DelayFastRetryAsync(TimeSpan.Zero);

    result.ShouldBeTrue();
  }

  [TestMethod]
  public async Task delay_fast_retry__cancelled_token__returns_false()
  {
    using var cancellationSource = new CancellationTokenSource();
    await cancellationSource.CancelAsync();

    var result = await DelayFastRetryAsync(TimeSpan.FromHours(1), cancellationSource.Token);

    result.ShouldBeFalse();
  }
}
