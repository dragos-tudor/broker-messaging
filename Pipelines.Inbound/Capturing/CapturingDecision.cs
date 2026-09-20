namespace Pipelines.Inbound;

internal readonly union CapturingDecision(
  CapturingActions,
  InboundPipelineTypes,
  TerminalActions
);
