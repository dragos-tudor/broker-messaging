
using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static DeadLetteringTransition AdvanceDeadLetteringPipeline(
    DeadLetteringSignal signal,
    InboundPipelineConfig _) => signal switch
  {
    DeadLetteringEntry.Start => DeadLetteringActions.Converting,

    ConvertingStates.Success => DeadLetteringActions.Inserting,
    ConvertingStates.Error => DeadLetteringActions.Abandoning,

    DeadLetter.InsertingStates.Success => DeadLetteringActions.Closing,
    DeadLetter.InsertingStates.Idempotent => DeadLetteringActions.Closing,
    DeadLetter.InsertingStates.Error => TerminalActions.Exit,

    AbandoningStates.Success => TerminalActions.Exit,
    AbandoningStates.Error => TerminalActions.Exit,

    ClosingStates.Success => InboundPipelineTypes.Publishing,
    ClosingStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}

