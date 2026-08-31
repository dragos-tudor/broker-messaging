using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string DeadLetterPipeline(string state) => state switch
  {
    InsertDeadLetterMessageSuccessState => DeadLetterActions.Mapping,
    InsertDeadLetterMessageErrorState => DeadLetterActions.Inserting,
    IdempotentDeadLetterMessageState => DeadLetterActions.Mapping,

    MapDeadLetterMessageSuccessState => DeadLetterActions.Mapped,
    MapDeadLetterMessageErrorState => DeadLetterActions.Unrecoverable,
    MapDeadLetterMessagePayloadErrorState => DeadLetterActions.Unrecoverable,

    ScheduleDeadLetterMessageExhaustedState => DeadLetterActions.Abandoning,
    ScheduleDeadLetterMessageRetryState => DeadLetterActions.Scheduled,
    ScheduleDeadLetterMessageErrorState => DeadLetterActions.Scheduling,

    AbandonDeadLetterMessageSuccessState => DeadLetterActions.Abandoned,
    AbandonDeadLetterMessageErrorState => DeadLetterActions.Abandoning,

    CloseDeadLetterMessageSuccessState => DeadLetterActions.Closed,
    CloseDeadLetterMessageErrorState => DeadLetterActions.Closing,

    _ => DeadLetterActions.Unknown
  };
}

internal static class DeadLetterActions
{
  private const string Scope = "DeadLetter";

  public const string Inserting = $"{Scope}.{nameof(Inserting)}";
  public const string Mapping = $"{Scope}.{nameof(Mapping)}";
  public const string Mapped = $"{Scope}.{nameof(Mapped)}";
  public const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  public const string Scheduled = $"{Scope}.{nameof(Scheduled)}";
  public const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  public const string Abandoned = $"{Scope}.{nameof(Abandoned)}";
  public const string Closing = $"{Scope}.{nameof(Closing)}";
  public const string Closed = $"{Scope}.{nameof(Closed)}";
  public const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
  public const string Unknown = $"{Scope}.{nameof(Unknown)}";
}