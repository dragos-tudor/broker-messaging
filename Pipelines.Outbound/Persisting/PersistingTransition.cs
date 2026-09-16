namespace Pipelines.Outbound;

internal readonly union PersistingTransition(
  PersistingActions,
  PipelinesTypes,
  TerminalActions
);
