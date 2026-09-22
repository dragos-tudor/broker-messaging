
namespace Routing.Inbound;

internal readonly union InboundRoutingDecision(
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
