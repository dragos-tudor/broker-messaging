
namespace Routing.Inbound;

internal readonly union InboundRoutingDecision(
  InboundPipelineTypes,
  TerminalActions
);