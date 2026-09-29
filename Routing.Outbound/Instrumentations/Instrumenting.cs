namespace Routing.Outbound;

partial class OutboundFuncs
{
  internal static void InstrumentPipeline(
    InstrumentationCapabilities capabilities,
    string signal,
    string decision,
    PipelineContext context) =>
      LogPipeline(
        capabilities.GetLogger(),
        signal,
        decision,
        context.CorrelationId,
        context.OutboxMessageId);

  internal static void InstrumentOperation(
    InstrumentationCapabilities capabilities,
    string signal,
    PipelineContext context,
    Exception? exception)
  {
    if (exception is null)
      InstrumentSuccessOperation(capabilities, signal, context);
    if (exception is not null)
      InstrumentErrorOperation(capabilities, signal, context, exception);
  }

  static void InstrumentSuccessOperation(
    InstrumentationCapabilities capabilities,
    string signal,
    PipelineContext context) =>
    LogOperation(
      capabilities.GetLogger(),
      signal,
      context.CorrelationId,
      context.OutboxMessageId);

  static void InstrumentErrorOperation(
    InstrumentationCapabilities capabilities,
    string signal,
    PipelineContext context,
    Exception exception) =>
    LogOperationError(
      capabilities.GetLogger(),
      signal,
      context.CorrelationId,
      context.OutboxMessageId,
      exception);
}
