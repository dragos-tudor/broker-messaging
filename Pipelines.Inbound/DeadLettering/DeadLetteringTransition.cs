namespace Pipelines.Inbound;

internal readonly union DeadLetteringTransition(
  DeadLetteringActions,
  InboundPipelineTypes,
  TerminalActions
);
