namespace Pipelines.Inbound;

internal readonly union RedirectingContinuation(
  RedirectingActions,
  PipelineTypes,
  TerminalActions
);
