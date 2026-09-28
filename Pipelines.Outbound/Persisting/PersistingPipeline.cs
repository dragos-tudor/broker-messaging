using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static string AdvancePersistingPipeline(
    string signal,
    PipelineConfig config) => signal switch
  {
    PersistingEntries.Start => PersistingActions.Validating,

    ValidatingStates.Success => PersistingActions.Transacting,
    ValidatingStates.InvalidError => TerminalActions.Exit,
    ValidatingStates.Error => TerminalActions.Exit,

    TransactingStates.Success => config.PublishAfterPersist ?
      PipelineTypes.Publishing :
      TerminalActions.Exit,
    TransactingStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}
