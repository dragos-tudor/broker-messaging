namespace Pipelines.Outbound;

internal readonly union PublishingDecision(
  PublishingActions,
  OutboundPipelinesTypes,
  TerminalActions
);
