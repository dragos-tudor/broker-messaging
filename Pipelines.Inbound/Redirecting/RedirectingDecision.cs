namespace Pipelines.Inbound;

internal readonly union RedirectingDecision(
  RedirectingActions,
  InboundPipelineTypes,
  TerminalActions
);
