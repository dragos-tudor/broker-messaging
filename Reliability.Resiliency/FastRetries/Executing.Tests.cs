using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Shouldly;

namespace Reliability.Resiliency;

[TestClass]
public partial class ResiliencyTests
{
  [TestMethod]
  public async Task execute_with_fast_retry__non_retryable_signal__returns_result_without_delay()
  {
    var services = CreateServices(new FastRetryOptions());
    var operationCount = 0;
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct)
    {
      operationCount++;
      return Task.FromResult((data, RetrySignal.Completed, (Exception?)null));
    }

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(true);
    }

    var result = await ResiliencyFuncs.ExecuteWithFastRetryAsync(
      services, "initial", "transition", ExecuteOperation, CanRetry, DelayRetryExecution);

    result.ShouldBe(("initial", RetrySignal.Completed, (Exception?)null));
    operationCount.ShouldBe(1);
    delayCount.ShouldBe(0);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__zero_retries__returns_first_result_without_delay()
  {
    var services = CreateServices(new FastRetryOptions { MaxRetryAttempts = 0 });
    var operationCount = 0;
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct)
    {
      operationCount++;
      return Task.FromResult((data, RetrySignal.Retry, (Exception?)null));
    }

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(true);
    }

    var result = await ResiliencyFuncs.ExecuteWithFastRetryAsync(
      services, "initial", "transition", ExecuteOperation, CanRetry, DelayRetryExecution);

    result.ShouldBe(("initial", RetrySignal.Retry, (Exception?)null));
    operationCount.ShouldBe(1);
    delayCount.ShouldBe(0);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__delay_declines_retry__returns_current_result()
  {
    var services = CreateServices(new FastRetryOptions());
    var operationCount = 0;
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct)
    {
      operationCount++;
      return Task.FromResult((data, RetrySignal.Retry, (Exception?)null));
    }

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(false);
    }

    var result = await ResiliencyFuncs.ExecuteWithFastRetryAsync(
      services, "initial", "transition", ExecuteOperation, CanRetry, DelayRetryExecution);

    result.ShouldBe(("initial", RetrySignal.Retry, (Exception?)null));
    operationCount.ShouldBe(1);
    delayCount.ShouldBe(1);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__retry_then_success__returns_second_result_and_uses_updated_data()
  {
    var services = CreateServices(new FastRetryOptions());
    var operationInputs = new List<string>();
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct)
    {
      operationInputs.Add(data);
      return Task.FromResult(operationInputs.Count is 1
        ? ("updated", RetrySignal.Retry, (Exception?)null)
        : ("completed", RetrySignal.Completed, (Exception?)null));
    }

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(true);
    }

    var result = await ResiliencyFuncs.ExecuteWithFastRetryAsync(
      services, "initial", "transition", ExecuteOperation, CanRetry, DelayRetryExecution);

    result.ShouldBe(("completed", RetrySignal.Completed, (Exception?)null));
    operationInputs.ShouldBe(["initial", "updated"]);
    delayCount.ShouldBe(1);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__retry_attempts_exhausted__returns_last_result()
  {
    var services = CreateServices(new FastRetryOptions { MaxRetryAttempts = 2 });
    var operationCount = 0;
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct)
    {
      operationCount++;
      return Task.FromResult(($"attempt-{operationCount}", RetrySignal.Retry, (Exception?)null));
    }

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(true);
    }

    var result = await ResiliencyFuncs.ExecuteWithFastRetryAsync(
      services, "initial", "transition", ExecuteOperation, CanRetry, DelayRetryExecution);

    result.ShouldBe(("attempt-3", RetrySignal.Retry, (Exception?)null));
    operationCount.ShouldBe(3);
    delayCount.ShouldBe(2);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__cancelled_after_operation__returns_result_without_retry_checks()
  {
    var services = CreateServices(new FastRetryOptions());
    using var cancellationSource = new CancellationTokenSource();
    var retryCheckCount = 0;
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct)
    {
      cancellationSource.Cancel();
      return Task.FromResult((data, RetrySignal.Retry, (Exception?)null));
    }

    bool CanRetrySignal(RetrySignal signal)
    {
      retryCheckCount++;
      return CanRetry(signal);
    }

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(true);
    }

    var result = await ResiliencyFuncs.ExecuteWithFastRetryAsync(
      services,
      "initial",
      "transition",
      ExecuteOperation,
      CanRetrySignal,
      DelayRetryExecution,
      cancellationSource.Token);

    result.ShouldBe(("initial", RetrySignal.Retry, (Exception?)null));
    retryCheckCount.ShouldBe(0);
    delayCount.ShouldBe(0);
  }

  [TestMethod]
  public async Task execute_with_fast_retry__operation_throws__propagates_exception_without_delay()
  {
    var services = CreateServices(new FastRetryOptions());
    var expectedException = new InvalidOperationException("operation failed");
    var delayCount = 0;

    Task<(string, RetrySignal, Exception?)> ExecuteOperation(
      string transition,
      IFastRetryOptionsService operationServices,
      string data,
      CancellationToken ct) =>
        Task.FromException<(string, RetrySignal, Exception?)>(expectedException);

    Task<bool> DelayRetryExecution(TimeSpan delay, CancellationToken ct)
    {
      delayCount++;
      return Task.FromResult(true);
    }

    var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
      ResiliencyFuncs.ExecuteWithFastRetryAsync(
        services,
        "initial",
        "transition",
        ExecuteOperation,
        CanRetry,
        DelayRetryExecution));

    exception.ShouldBeSameAs(expectedException);
    delayCount.ShouldBe(0);
  }

  static IFastRetryOptionsService CreateServices(FastRetryOptions options)
  {
    var services = Substitute.For<IFastRetryOptionsService>();
    services.GetFastRetryOptions().Returns(options);
    return services;
  }

  static bool CanRetry(RetrySignal signal) => signal is RetrySignal.Retry;

  enum RetrySignal
  {
    Completed,
    Retry
  }
}
