#pragma warning disable CS4014

namespace Reliability.Resiliency;

partial class ResiliencyTests
{
  [TestMethod]
  public async Task execute_with_fast_retry__non_retryable_signal__returns_result_without_delay()
  {
    var services = CreateServices(new FastRetryOptions());
    var executeOperation = CreateExecuteOperation(
      ("initial", RetrySignal.Completed, null));
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(true);

    var result = await ExecuteWithFastRetryAsync(
      services, "initial", "transition", executeOperation, canRetry, delayRetryExecution);

    result.ShouldBe(("initial", RetrySignal.Completed, null));
    executeOperation.Received(1).Invoke(
      "transition", services, "initial", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Completed);
    delayRetryExecution.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task execute_with_fast_retry__zero_retries__returns_first_result_without_delay()
  {
    var services = CreateServices(new FastRetryOptions { MaxRetryAttempts = 0 });
    var executeOperation = CreateExecuteOperation(
      ("initial", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(true);

    var result = await ExecuteWithFastRetryAsync(
      services, "initial", "transition", executeOperation, canRetry, delayRetryExecution);

    result.ShouldBe(("initial", RetrySignal.Retry, null));
    executeOperation.Received(1).Invoke(
      "transition", services, "initial", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Retry);
    delayRetryExecution.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task execute_with_fast_retry__delay_declines_retry__returns_current_result()
  {
    var services = CreateServices(new FastRetryOptions());
    var executeOperation = CreateExecuteOperation(
      ("initial", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(false);

    var result = await ExecuteWithFastRetryAsync(
      services, "initial", "transition", executeOperation, canRetry, delayRetryExecution);

    result.ShouldBe(("initial", RetrySignal.Retry, null));
    executeOperation.Received(1).Invoke(
      "transition", services, "initial", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Retry);
    delayRetryExecution.Received(1).Invoke(Arg.Any<TimeSpan>(), CancellationToken.None);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__retry_then_success__returns_second_result_and_uses_updated_data()
  {
    var services = CreateServices(new FastRetryOptions());
    var executeOperation = CreateExecuteOperation(
      ("updated", RetrySignal.Retry, null),
      ("completed", RetrySignal.Completed, null));
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(true);

    var result = await ExecuteWithFastRetryAsync(
      services, "initial", "transition", executeOperation, canRetry, delayRetryExecution);

    result.ShouldBe(("completed", RetrySignal.Completed, null));
    executeOperation.Received(1).Invoke(
      "transition", services, "initial", CancellationToken.None);
    executeOperation.Received(1).Invoke(
      "transition", services, "updated", CancellationToken.None);
    canRetry.Received(1).Invoke(RetrySignal.Retry);
    canRetry.Received(1).Invoke(RetrySignal.Completed);
    delayRetryExecution.Received(1).Invoke(Arg.Any<TimeSpan>(), CancellationToken.None);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__retry_attempts_exhausted__returns_last_result()
  {
    var services = CreateServices(new FastRetryOptions { MaxRetryAttempts = 2 });
    var executeOperation = CreateExecuteOperation(
      ("attempt-1", RetrySignal.Retry, null),
      ("attempt-2", RetrySignal.Retry, null),
      ("attempt-3", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(true);

    var result = await ExecuteWithFastRetryAsync(
      services, "initial", "transition", executeOperation, canRetry, delayRetryExecution);

    result.ShouldBe(("attempt-3", RetrySignal.Retry, null));
    executeOperation.Received(3).Invoke(
      Arg.Any<string>(), services, Arg.Any<string>(), CancellationToken.None);
    canRetry.Received(3).Invoke(RetrySignal.Retry);
    delayRetryExecution.Received(2).Invoke(Arg.Any<TimeSpan>(), CancellationToken.None);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__cancelled_after_operation__returns_result_without_retry_checks()
  {
    var services = CreateServices(new FastRetryOptions());
    using var cancellationSource = new CancellationTokenSource();
    var executeOperation = CreateExecuteOperation(
      () => cancellationSource.Cancel(),
      ("initial", RetrySignal.Retry, null));
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(true);

    var result = await ExecuteWithFastRetryAsync(
      services,
      "initial",
      "transition",
      executeOperation,
      canRetry,
      delayRetryExecution,
      cancellationSource.Token);

    result.ShouldBe(("initial", RetrySignal.Retry, null));
    executeOperation.Received(1).Invoke(
      "transition", services, "initial", cancellationSource.Token);
    canRetry.DidNotReceive().Invoke(Arg.Any<RetrySignal>());
    delayRetryExecution.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task execute_with_fast_retry__operation_throws__propagates_exception_without_delay()
  {
    var services = CreateServices(new FastRetryOptions());
    var expectedException = new InvalidOperationException("operation failed");
    var executeOperation = CreateExecuteOperation(() => { throw expectedException; });
    var canRetry = CreateCanRetry();
    var delayRetryExecution = CreateDelayRetryExecution(true);

    var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
      ExecuteWithFastRetryAsync(
        services,
        "initial",
        "transition",
        executeOperation,
        canRetry,
        delayRetryExecution));

    exception.ShouldBeSameAs(expectedException);
    executeOperation.Received(1).Invoke(
      "transition", services, "initial", CancellationToken.None);
    canRetry.DidNotReceive().Invoke(Arg.Any<RetrySignal>());
    delayRetryExecution.DidNotReceive().Invoke(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
  }
}
