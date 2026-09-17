namespace Pipelines.Outbound;

internal readonly union PersistingTransition(
  PersistingActions,
  OutboundPipelinesTypes,
  TerminalActions
);
