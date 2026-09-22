
namespace Routing.Inbound;

public enum TestStates { Initial, Retry, Completed }

internal readonly union TestSignal(TestStates);

internal readonly union TestDecision(InboundPipelineTypes, TerminalActions) : IInboundDecision
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