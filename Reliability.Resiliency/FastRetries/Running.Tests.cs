#pragma warning disable CS4014

namespace Reliability.Resiliency;

partial class ResiliencyTests
{
  [TestMethod]
  public async Task run_with_fast_retry__non_retryable_signal__returns_result_without_delay()
  {
    var services = CreateServices(new FastRetryOptions());
    var executeOperation = CreateExecuteOperation(
      ("initial", RetrySignal.Completed, null));
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(true);

    var result = await RunFastRetryAsync(
      services, "initial", "decision", executeOperation, canRetry, delayFastRetry);

    result.ShouldBe(("initial", RetrySignal.Completed, null));
    executeOperation.Received(1).Invoke(
      "decision", services, "initial", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Completed);
    delayFastRetry.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task run_with_fast_retry__zero_retries__returns_first_result_without_delay()
  {
    var services = CreateServices(new FastRetryOptions { MaxRetryAttempts = 0 });
    var executeOperation = CreateExecuteOperation(
      ("initial", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(true);

    var result = await RunFastRetryAsync(
      services, "initial", "decision", executeOperation, canRetry, delayFastRetry);

    result.ShouldBe(("initial", RetrySignal.Retry, null));
    executeOperation.Received(1).Invoke(
      "decision", services, "initial", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Retry);
    delayFastRetry.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task run_with_fast_retry__delay_declines_retry__returns_current_result()
  {
    var services = CreateServices(new FastRetryOptions());
    var executeOperation = CreateExecuteOperation(
      ("initial", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(false);

    var result = await RunFastRetryAsync(
      services, "initial", "decision", executeOperation, canRetry, delayFastRetry);

    result.ShouldBe(("initial", RetrySignal.Retry, null));
    executeOperation.Received(1).Invoke(
      "decision", services, "initial", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Retry);
    delayFastRetry.Received(1).Invoke(Arg.Any<TimeSpan>(), CancellationToken.None);
  }

  [TestMethod]
  public async Task run_with_fast_retry__retry_then_success__returns_second_result_and_uses_updated_data()
  {
    var services = CreateServices(new FastRetryOptions());
    var executeOperation = CreateExecuteOperation(
      ("updated", RetrySignal.Retry, null),
      ("completed", RetrySignal.Completed, null));
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(true);

    var result = await RunFastRetryAsync(
      services, "initial", "decision", executeOperation, canRetry, delayFastRetry);

    result.ShouldBe(("completed", RetrySignal.Completed, null));
    executeOperation.Received(1).Invoke(
      "decision", services, "initial", CancellationToken.None);
    executeOperation.Received(1).Invoke(
      "decision", services, "updated", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Retry);
    canRetry.Received(1).Invoke(RetrySignal.Completed);
    delayFastRetry.Received(1).Invoke(Arg.Any<TimeSpan>(), CancellationToken.None);
  }

  [TestMethod]
  public async Task run_with_fast_retry__retry_attempts_exhausted__returns_last_result()
  {
    var services = CreateServices(new FastRetryOptions { MaxRetryAttempts = 2 });
    var executeOperation = CreateExecuteOperation(
      ("attempt-1", RetrySignal.Retry, null),
      ("attempt-2", RetrySignal.Retry, null),
      ("attempt-3", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(true);

    var result = await RunFastRetryAsync(
      services, "initial", "decision", executeOperation, canRetry, delayFastRetry);

    result.ShouldBe(("attempt-3", RetrySignal.Retry, null));
    executeOperation.Received(3).Invoke(
      Arg.Any<string>(), services, Arg.Any<string>(), CancellationToken.None);
    canRetry.Received(3).Invoke(RetrySignal.Retry);
    delayFastRetry.Received(2).Invoke(Arg.Any<TimeSpan>(), CancellationToken.None);
  }

  [TestMethod]
  public async Task run_with_fast_retry__cancelled_after_operation__returns_result_without_retry_checks()
  {
    var services = CreateServices(new FastRetryOptions());
    using var cancellationSource = new CancellationTokenSource();
    var executeOperation = CreateExecuteOperation(
      () => cancellationSource.Cancel(),
      ("initial", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(true);

    var result = await RunFastRetryAsync(
      services,
      "initial",
      "decision",
      executeOperation,
      canRetry,
      delayFastRetry,
      cancellationSource.Token);

    result.ShouldBe(("initial", RetrySignal.Retry, null));
    executeOperation.Received(1).Invoke(
      "decision", services, "initial", cancellationSource.Token);
    canRetry.DidNotReceive().Invoke(Arg.Any<RetrySignal>());
    delayFastRetry.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task run_with_fast_retry__operation_throws__propagates_exception_without_delay()
  {
    var services = CreateServices(new FastRetryOptions());
    var expectedException = new InvalidOperationException("operation failed");
    var executeOperation = CreateExecuteOperation(() => { throw expectedException; });
    var canRetry = CreateCanRetry();
    var delayFastRetry = CreateDelayFastRetry(true);

    var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
      RunFastRetryAsync(
        services,
        "initial",
        "decision",
        executeOperation,
        canRetry,
        delayFastRetry));

    exception.ShouldBeSameAs(expectedException);
    executeOperation.Received(1).Invoke(
      "decision", services, "initial", CancellationToken.None);
    canRetry.DidNotReceive().Invoke(Arg.Any<RetrySignal>());
    delayFastRetry.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }
}
