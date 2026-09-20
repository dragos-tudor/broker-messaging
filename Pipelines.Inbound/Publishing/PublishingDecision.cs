namespace Pipelines.Inbound;

internal readonly union PublishingDecision(
  PublishingActions,
  InboundPipelineTypes,
  TerminalActions
);
