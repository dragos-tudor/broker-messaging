
namespace Routing.Inbound;

public readonly union InboundRoutingDecision(
  InboundPipelineTypes,
  TerminalActions
);
