
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  [LoggerMessage(1, LogLevel.Error, "Periodic job error. Job name {JobName}")]
  static partial void LogPeriodicJobError(ILogger logger, string jobName, Exception exception);
}