namespace Pipelines.Inbound;

internal readonly union HandlingDecision(
  HandlingActions,
  InboundPipelineTypes,
  TerminalActions
);
