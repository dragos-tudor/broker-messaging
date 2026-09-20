namespace Pipelines.Outbound;

internal readonly union DispatchingDecision(
  DispatchingActions,
  OutboundPipelinesTypes,
  TerminalActions
);
