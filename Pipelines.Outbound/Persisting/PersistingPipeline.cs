
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static string? PersistingPipeline(string state, OutboundPipelineConfig config) => state switch
  {
    PipelinesTypes.Persisting => PersistingActions.Validating,

    OutboxStates.ValidatingSuccess => PersistingActions.Transacting,
    OutboxStates.ValidatingInvalidError => TerminalActions.Exit,
    OutboxStates.ValidatingError => TerminalActions.Exit,

    OutboxStates.TransactingSuccess => config.PublishAfterPersist?
      PipelinesTypes.Publishing:
      TerminalActions.Exit,
    OutboxStates.TransactingError => TerminalActions.Exit,

    _ => default
  };
}
