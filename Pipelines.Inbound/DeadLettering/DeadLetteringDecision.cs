namespace Pipelines.Inbound;

internal readonly union DeadLetteringDecision(
  DeadLetteringActions,
  InboundPipelineTypes,
  TerminalActions
);
