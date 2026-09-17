namespace Pipelines.Inbound;

internal readonly union CapturingTransition(
  CapturingActions,
  InboundPipelineTypes,
  TerminalActions
);
