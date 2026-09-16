namespace Pipelines.Inbound;

internal readonly union HandlingTransition(
  HandlingActions,
  PipelineTypes,
  TerminalActions
);
