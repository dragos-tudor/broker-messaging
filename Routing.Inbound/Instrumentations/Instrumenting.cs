
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static void InstrumentPipeline(
    InstrumentationCapabilities capabilities,
    string signal,

    PipelineContext context) =>
      LogPipeline(
        capabilities.GetLogger(),
        signal,
        context.TransportId,
        context.CorrelationId,
        context.InboxMessageId,
        context.DeadLetterMessageId);

  internal static void InstrumentOperation(
    InstrumentationCapabilities capabilities,
    string signal,
    string decision,
    PipelineContext context,
    Exception? exception)
  {
    if (exception is null)
      InstrumentSuccessOperation(capabilities, signal, decision, context);
    if (exception is not null)
      InstrumentErrorOperation(capabilities, signal, decision, context, exception);
  }

  static void InstrumentSuccessOperation(
    InstrumentationCapabilities capabilities,
    string signal,
    string decision,
    PipelineContext context) =>
    LogOperation(
      capabilities.GetLogger(),
      signal,
      decision,
      context.TransportId,
      context.CorrelationId,
      context.InboxMessageId,
      context.DeadLetterMessageId);

  static void InstrumentErrorOperation(
    InstrumentationCapabilities capabilities,
    string signal,
    string decision,
    PipelineContext context,
    Exception exception) =>
    LogOperationError(
      capabilities.GetLogger(),
      signal,
      decision,
      context.TransportId,
      context.CorrelationId,
      context.InboxMessageId,
      context.DeadLetterMessageId,
      exception);

  /* TODO
  Router/retry logs
    retry attempt
    retry exhaustion
    delay/recovery decision

  Activity
    TraceId / SpanId
    timing
    parent-child relationship

  Metrics
    counts
    duration distributions
    retry counts
    failures
  */
}