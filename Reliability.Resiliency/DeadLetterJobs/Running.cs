
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static Task RunDeadLetterJobAsync<TKey, TValue, TPayload>(
    DeadLetterJobOptions options,
    IPeriodicJobServices services,
    CancellationToken ct = default) =>
      RunPeriodicJobAsync(
        "deadletter.job",
        options.DeadLetterJobRunInterval,
        options.DeadLetterJobLockInterval,
        ct => Task.CompletedTask,
        services,
        ct);
}
