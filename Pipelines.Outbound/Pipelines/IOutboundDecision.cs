
namespace Pipelines.Outbound;

internal interface IOutboundDecision
{
  OutboundPipelineTypes GetPipelineType();
  TerminalActions GetTerminalAction();
}