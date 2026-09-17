namespace Pipelines.Inbound;

internal readonly union HandlingTransition(
  HandlingActions,
  InboundPipelineTypes,
  TerminalActions
);
