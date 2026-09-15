namespace Pipelines.Inbound;

internal readonly union HandlingContinuation(
  HandlingActions,
  PipelineTypes,
  TerminalActions
);
