using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetDeadLetterPipelineAction(string state) => state switch
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

    _ => default
  };
}