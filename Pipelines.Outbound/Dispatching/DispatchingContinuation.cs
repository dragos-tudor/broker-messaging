namespace Pipelines.Outbound;

internal readonly union DispatchingContinuation(
  DispatchingActions,
  PipelinesTypes,
  TerminalActions
);
