namespace Pipelines.Outbound;

internal readonly union PublishingDecision(
  PublishingActions,
  OutboundPipelineTypes,
  TerminalActions
) : IOutboundDecision
{
  public OutboundPipelineTypes GetPipelineType() => this switch
  {
    OutboundPipelineTypes pipelineType => pipelineType,
    _ => OutboundPipelineTypes.None
  };

  public TerminalActions GetTerminalAction() => this switch
  {
    TerminalActions terminalAction => terminalAction,
    _ => TerminalActions.None
  };
}
