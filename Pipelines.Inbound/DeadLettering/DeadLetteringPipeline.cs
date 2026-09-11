
using Operations.Inbound.DeadLetter;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? MapDeadLetteringAction(string state, InboundPipelineConfig _) => state switch
  {
    InboxStates.ConvertingSuccess => DeadLetteringActions.Inserting,
    InboxStates.ConvertingError => DeadLetteringActions.Abandoning,

    DeadLetterStates.InsertingSuccess => DeadLetteringActions.Closing,
    DeadLetterStates.InsertingIdempotent => DeadLetteringActions.Closing,
    DeadLetterStates.InsertingError => TerminalActions.Exit,

    InboxStates.AbandoningSuccess => TerminalActions.Exit,
    InboxStates.AbandoningError => TerminalActions.Exit,

    InboxStates.ClosingSuccess => InboundPipelines.Publishing,
    InboxStates.ClosingError => TerminalActions.Exit,

    _ => default
  };
}
