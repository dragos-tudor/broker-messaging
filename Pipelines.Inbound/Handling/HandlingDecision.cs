namespace Pipelines.Inbound;

internal readonly union HandlingDecision(
  HandlingActions,
  InboundPipelineTypes,
  TerminalActions
) : IInboundDecision
{
  public InboundPipelineTypes GetPipelineType() => this switch
  {
    InboundPipelineTypes pipelineType => pipelineType,
    _ => InboundPipelineTypes.None
  };

  public TerminalActions GetTerminalAction() => this switch
  {
    TerminalActions terminalAction => terminalAction,
    _ => TerminalActions.None
  };
}
