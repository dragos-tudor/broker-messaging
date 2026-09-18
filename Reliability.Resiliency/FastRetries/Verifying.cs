
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  static bool IsRetryExecutionExhausted(int retryCount, FastRetryOptions options) =>
    retryCount >= options.MaxRetryAttempts;
}