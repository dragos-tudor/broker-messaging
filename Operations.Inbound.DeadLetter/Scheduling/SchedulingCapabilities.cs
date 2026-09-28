namespace Operations.Inbound.DeadLetter;

public sealed record SchedulingCapabilities(
  UpdateDeadLetterMessageAsync<SchedulingUpdate>
    UpdateDeadLetterMessageAsync,
  GetDeadLetterRetryOptions GetDeadLetterRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
