namespace Pipelines.Inbound;

internal readonly union DeadLetteringContinuation(
  DeadLetteringActions,
  PipelineTypes,
  TerminalActions
);
