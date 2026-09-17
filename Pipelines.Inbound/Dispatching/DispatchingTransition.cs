namespace Pipelines.Inbound;

internal readonly union DispatchingTransition(
  DispatchingActions,
  InboundPipelineTypes,
  TerminalActions
);
