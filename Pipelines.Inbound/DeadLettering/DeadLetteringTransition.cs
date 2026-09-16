namespace Pipelines.Inbound;

internal readonly union DeadLetteringTransition(
  DeadLetteringActions,
  PipelineTypes,
  TerminalActions
);
