namespace Pipelines.Outbound;

internal readonly union PersistingDecision(
  PersistingActions,
  OutboundPipelinesTypes,
  TerminalActions
);
