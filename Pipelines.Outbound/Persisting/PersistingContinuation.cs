namespace Pipelines.Outbound;

internal readonly union PersistingContinuation(
  PersistingActions,
  PipelinesTypes,
  TerminalActions
);
