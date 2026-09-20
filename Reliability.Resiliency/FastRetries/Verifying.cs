
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  static bool IsFastRetryExhausted(int retryCount, FastRetryOptions options) =>
    retryCount >= options.MaxRetryAttempts;
}