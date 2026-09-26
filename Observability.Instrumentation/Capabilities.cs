
namespace Observability.Instrumentation;

public sealed record InstrumentationCapabilities
(
  GetLogger GetLogger,
  GetActivitySource GetActivitySource,
  GetMetricCounter GetMetricCounter
);