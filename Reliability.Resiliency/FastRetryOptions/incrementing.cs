namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static int IncrementNextRetryCount(int retryCount) =>
    retryCount + 1;
}
