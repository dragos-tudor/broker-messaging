using Services = Routing.Inbound.IInboundRoutingServices<string, int, string, string, byte[], System.IDisposable>;
using Data = Routing.Inbound.IInboundRoutingData<string, int, string, string, byte[]>;

namespace Routing.Inbound;

partial class IntegrationTests
{
  static void ConfigureFastRetryOptions(Services services, FastRetryOptions? options = null)
  {
    services.GetFastRetryOptions().Returns(options ?? new FastRetryOptions() { MaxRetryAttempts = 0 });
  }

  static void ConfigureInboundPipelineConfig(Services services)
  {
    services.GetInboundPipelineConfig().Returns(new InboundPipelineConfig());
  }

   static void ConfigureOperationsInstrumentations(List<(string, Enum, string?)> operationLogs, Services services)
  {
    ConfigureOperationInstrumentation<CapturingSignal>(operationLogs, services);
    ConfigureOperationInstrumentation<RedirectingSignal>(operationLogs, services);
    ConfigureOperationInstrumentation<HandlingSignal>(operationLogs, services);
    ConfigureOperationInstrumentation<DeadLetteringSignal>(operationLogs, services);
    ConfigureOperationInstrumentation<PublishingSignal>(operationLogs, services);
    ConfigureOperationInstrumentation<DispatchingSignal>(operationLogs, services);
  }

  static void ConfigureOperationInstrumentation<TSignal>(List<(string, Enum, string?)> operationLogs, Services services)
    where TSignal: struct
  {
    services.When((services) => services.InstrumentOperation(services, Arg.Any<Data>(), Arg.Any<TSignal>(), Arg.Any<Exception?>()))
      .Do(args => operationLogs.Add(("data", FromSignal(args[2]), ((Exception?)args[3])?.Message)));
  }

  static void ConfigurePipelineInstrumentations(List<(Enum, Enum)> pipelineLogs, Services services)
  {
    ConfigurePipelineInstrumentation<CapturingSignal, CapturingDecision>(pipelineLogs, services);
    ConfigurePipelineInstrumentation<RedirectingSignal, RedirectingDecision>(pipelineLogs, services);
    ConfigurePipelineInstrumentation<HandlingSignal, HandlingDecision>(pipelineLogs, services);
    ConfigurePipelineInstrumentation<DeadLetteringSignal, DeadLetteringDecision>(pipelineLogs, services);
    ConfigurePipelineInstrumentation<PublishingSignal, PublishingDecision>(pipelineLogs, services);
    ConfigurePipelineInstrumentation<DispatchingSignal, DispatchingDecision>(pipelineLogs, services);
  }

  static void ConfigurePipelineInstrumentation<TSignal, TDecision>(List<(Enum, Enum)> pipelineLogs, Services services)
    where TSignal: struct
    where TDecision: struct
  {
    services.When((services) => services.InstrumentPipeline(services, Arg.Any<TSignal>(), Arg.Any<TDecision>()))
      .Do(args => pipelineLogs.Add((FromSignal(args[1]), FromDecision(args[2]))));
  }
}
