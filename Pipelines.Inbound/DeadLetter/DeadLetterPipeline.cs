using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetDeadLetterPipelineAction(string state) => state switch
  {
    DeadLetterStates.InsertingSuccess => DeadLetterActions.Mapping,
    DeadLetterStates.InsertingError => DeadLetterActions.Inserting,
    DeadLetterStates.Idempotent => DeadLetterActions.Mapping,

    DeadLetterStates.MappingSuccess => DeadLetterActions.Mapped,
    DeadLetterStates.MappingError => TerminalActions.Unrecoverable,
    DeadLetterStates.MappingPayloadError => TerminalActions.Unrecoverable,

    DeadLetterStates.SchedulingExhausted => DeadLetterActions.Abandoning,
    DeadLetterStates.SchedulingNotExhausted => DeadLetterActions.Scheduled,
    DeadLetterStates.SchedulingError => DeadLetterActions.Scheduling,

    DeadLetterStates.AbandoningSuccess => DeadLetterActions.Abandoned,
    DeadLetterStates.AbandoningError => DeadLetterActions.Abandoning,

    DeadLetterStates.ClosingSuccess => DeadLetterActions.Closed,
    DeadLetterStates.ClosingError => DeadLetterActions.Closing,

    _ => default
  };
}
