namespace Pipelines.Outbound;

internal readonly union DispatchingTransition(
  DispatchingActions,
  PipelinesTypes,
  TerminalActions
);
