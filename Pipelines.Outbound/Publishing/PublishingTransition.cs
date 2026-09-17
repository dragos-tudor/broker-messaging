namespace Pipelines.Outbound;

internal readonly union PublishingTransition(
  PublishingActions,
  OutboundPipelinesTypes,
  TerminalActions
);
