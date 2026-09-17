namespace Pipelines.Inbound;

internal readonly union PublishingTransition(
  PublishingActions,
  InboundPipelineTypes,
  TerminalActions
);
