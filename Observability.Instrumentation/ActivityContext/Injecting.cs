
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  // manually inject the traceparent header from the activity context
  // TODO inject into baggage header as well
  // internal static Headers InjectTraceParentActivity(
  //   Activity? activity,
  //   Headers headers) =>
  //     activity is not null?
  //       SetTraceParentKafkaHeader(headers, ToTraceParent(activity)):
  //       headers;

  // delegate to OpenTelemetry TraceContextPropagator to inject the traceparent header from the activity context
  // [Obsolete("Too many allocations for traceparent injection (CreatePropagationContext)")]
  // static void InjectMessageActivity(
  //   Activity activity,
  //   Headers headers,
  //   TextMapPropagator? propagator = default) =>
  //     (propagator ?? Propagators.DefaultTextMapPropagator)
  //       .Inject(
  //         default, // CreatePropagationContext(activity),
  //         headers,
  //         SetTraceParentKafkaHeader(ToTraceParent(activity)));

}