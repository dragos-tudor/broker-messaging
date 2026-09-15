namespace Pipelines.Outbound;

internal readonly union PublishingContinuation(
  PublishingActions,
  PipelinesTypes,
  TerminalActions
);
