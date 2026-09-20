namespace Pipelines.Inbound;

internal readonly union DispatchingDecision(
  DispatchingActions,
  InboundPipelineTypes,
  TerminalActions
);
