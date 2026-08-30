
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  // manually extract the activity context from the traceparent header
  // TODO extract from baggage header as well
  // internal static ActivityContext? ExtractTraceParentActivityContext(Headers headers) =>
  //   GetTraceParentKafkaHeader(headers) is string traceParent?
  //     ToActivityContext(traceParent) : default;

  // delegate to OpenTelemetry TraceContextPropagator to extract the activity context from the traceparent header
  // [Obsolete("Too many allocations for traceparent extraction (CreatePropagationContext)")]
  // static ActivityContext ExtractMessageActivityContext<TPropagator>(
  //   Activity activity,
  //   Headers headers,
  //   TPropagator? propagator = default)
  //   where TPropagator : TextMapPropagator =>
  //     (propagator ?? Propagators.DefaultTextMapPropagator)
  //       .Extract(
  //         default, //CreatePropagationContext(activity),
  //         headers,
  //         (headers, name) => ToEnumerable(GetKafkaHeaderString(headers, name)))
  //       .ActivityContext;
}