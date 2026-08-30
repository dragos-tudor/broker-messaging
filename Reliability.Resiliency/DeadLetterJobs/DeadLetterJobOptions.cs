
namespace Reliability.Resiliency;

public record DeadLetterJobOptions
{
  public TimeSpan DeadLetterJobRunInterval { get; init; } = TimeSpan.FromSeconds(10);
  public TimeSpan DeadLetterJobLockInterval { get; init; } = TimeSpan.FromSeconds(30);
}

partial class ResiliencyFuncs
{
  public static DeadLetterJobOptions? CreateDeadLetterOptions(IConfiguration configuration) =>
    configuration.GetSection("Messaging").Get<DeadLetterJobOptions>();
}
