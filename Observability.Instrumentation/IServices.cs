
namespace Observability.Instrumentation;

public interface IInstrumentationServices:
  ITracingService,
  ILoggerService;

public interface ITracingService
{
  ActivitySource GetActivitySource();
}

public interface ILoggerService
{
  ILogger GetLogger();
}