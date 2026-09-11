
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static string? GetPersistingAction(string state, OutboundPipelineConfig config) => state switch
  {
    OutboxStates.ValidatingSuccess => PersistingActions.Transacting,
    OutboxStates.ValidatingInvalidError => TerminalActions.Exit,
    OutboxStates.ValidatingError => TerminalActions.Exit,

    OutboxStates.TransactingSuccess => config.PublishAfterPersist?
      OutboundPipelines.Publishing:
      TerminalActions.Exit,
    OutboxStates.TransactingError => TerminalActions.Exit,

    _ => default
  };
}
