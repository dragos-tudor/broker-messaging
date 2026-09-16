namespace Pipelines.Inbound;

internal readonly union DispatchingTransition(
  DispatchingActions,
  PipelineTypes,
  TerminalActions
);
