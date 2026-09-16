namespace Pipelines.Inbound;

internal readonly union CapturingTransition(
  CapturingActions,
  PipelineTypes,
  TerminalActions
);
