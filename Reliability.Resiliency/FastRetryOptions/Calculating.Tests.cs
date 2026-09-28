namespace Reliability.Resiliency;

partial class ResiliencyTests
{
  [TestMethod]
  public void calculate_next_attempt_delay__applies_backoff_factor()
  {
    var options = new FastRetryOptions
    {
      RetryBaseDelay = TimeSpan.FromMilliseconds(100),
      RetryBackoffFactor = 2,
      MaxRetryDelay = TimeSpan.FromSeconds(10)
    };

    var result = CalculateNextAttemptDelay(2, options);

    result.ShouldBe(TimeSpan.FromMilliseconds(400));
  }

  [TestMethod]
  public void calculate_next_attempt_delay__caps_at_maximum_delay()
  {
    var options = new FastRetryOptions
    {
      RetryBaseDelay = TimeSpan.FromMilliseconds(100),
      RetryBackoffFactor = 2,
      MaxRetryDelay = TimeSpan.FromMilliseconds(500)
    };

    var result = CalculateNextAttemptDelay(3, options);

    result.ShouldBe(options.MaxRetryDelay);
  }

  [TestMethod]
  public void calculate_next_retry_interval__interval_under_maximum__keeps_interval()
  {
    var options = new FastRetryOptions { MaxRetryDelay = TimeSpan.FromSeconds(2) };
    var interval = TimeSpan.FromSeconds(1);

    var result = CalculateNextRetryInterval(interval, options);

    result.ShouldBe(interval);
  }

  [TestMethod]
  public void calculate_next_retry_interval__interval_over_maximum__returns_maximum()
  {
    var options = new FastRetryOptions { MaxRetryDelay = TimeSpan.FromSeconds(2) };

    var result = CalculateNextRetryInterval(TimeSpan.FromSeconds(3), options);

    result.ShouldBe(options.MaxRetryDelay);
  }
}
