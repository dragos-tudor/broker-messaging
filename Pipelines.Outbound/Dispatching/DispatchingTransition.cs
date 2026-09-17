namespace Pipelines.Outbound;

internal readonly union DispatchingTransition(
  DispatchingActions,
  OutboundPipelinesTypes,
  TerminalActions
);
