namespace Pipelines.Outbound;

internal readonly union PublishingTransition(
  PublishingActions,
  PipelinesTypes,
  TerminalActions
);
