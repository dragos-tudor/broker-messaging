namespace Pipelines.Inbound;

internal readonly union PublishingContinuation(
  PublishingActions,
  PipelineTypes,
  TerminalActions
);
