
namespace Pipelines.Inbound;

internal interface IInboundDecision
{
  InboundPipelineTypes GetPipelineType();
  TerminalActions GetTerminalAction();
}