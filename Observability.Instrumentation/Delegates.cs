
namespace Observability.Instrumentation;

public delegate Counter<long> GetMetricCounter();

public delegate ActivitySource GetActivitySource();

public delegate ILogger GetLogger();
