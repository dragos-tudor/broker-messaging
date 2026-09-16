namespace Pipelines.Inbound;

internal readonly union PublishingTransition(
  PublishingActions,
  PipelineTypes,
  TerminalActions
);
