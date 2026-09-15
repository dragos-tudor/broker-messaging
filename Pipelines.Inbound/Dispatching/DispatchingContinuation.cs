namespace Pipelines.Inbound;

internal readonly union DispatchingContinuation(
  DispatchingActions,
  PipelineTypes,
  TerminalActions
);
