namespace Pipelines.Inbound;

internal readonly union RedirectingTransition(
  RedirectingActions,
  InboundPipelineTypes,
  TerminalActions
);
