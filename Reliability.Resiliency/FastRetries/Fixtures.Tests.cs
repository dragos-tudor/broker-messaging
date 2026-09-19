
namespace Reliability.Resiliency;

partial class ResiliencyTests
{
  public enum RetrySignal
  {
    Completed,
    Retry
  }

  static IFastRetryOptionsService CreateServices(FastRetryOptions options)
  {
    var services = Substitute.For<IFastRetryOptionsService>();
    services.GetFastRetryOptions().Returns(options);
    return services;
  }

  static Func<string, IFastRetryOptionsService, string, CancellationToken, Task<(string, RetrySignal, Exception?)>>
    CreateExecuteOperation(params (string, RetrySignal, Exception?)[] results) =>
      CreateExecuteOperation(null, results);

  static Func<string, IFastRetryOptionsService, string, CancellationToken, Task<(string, RetrySignal, Exception?)>>
    CreateExecuteOperation(
      Action? beforeExecute,
      params (string, RetrySignal, Exception?)[] results)
  {
    var executeOperation = Substitute.For<Func<string, IFastRetryOptionsService, string, CancellationToken, Task<(string, RetrySignal, Exception?)>>>();
    var resultIndex = 0;
    executeOperation(
      Arg.Any<string>(), Arg.Any<IFastRetryOptionsService>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
      .Returns(_ =>
      {
        beforeExecute?.Invoke();
        return Task.FromResult(results[resultIndex++]);
      });
    return executeOperation;
  }

  static Func<RetrySignal, bool> CreateCanRetry()
  {
    var canRetry = Substitute.For<Func<RetrySignal, bool>>();
    canRetry(RetrySignal.Completed).Returns(false);
    canRetry(RetrySignal.Retry).Returns(true);
    return canRetry;
  }

  static Func<TimeSpan, CancellationToken, Task<bool>> CreateDelayRetryExecution(bool result)
  {
    var delayRetryExecution = Substitute.For<Func<TimeSpan, CancellationToken, Task<bool>>>();
    delayRetryExecution(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(result));
    return delayRetryExecution;
  }
}