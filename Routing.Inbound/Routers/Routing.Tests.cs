#pragma warning disable CS4014

using Services = Routing.Inbound.IInboundRunningServices<string, string, string, string, string, Routing.Inbound.TestSession>;
using Data = Routing.Inbound.TestData;

namespace Routing.Inbound;

partial class InboundTests
{
  [TestMethod]
  public async Task route_inbound_pipelines__terminal_decision__returns_action()
  {
    var services = CreateServices();
    var data = new Data();
    var routePipeline = Substitute.For<Func<Services, Data, InboundPipelineTypes, CancellationToken, Task<(Data, InboundRoutingDecision)>>>();
    routePipeline(services, data, InboundPipelineTypes.Handling, CancellationToken.None)
      .Returns(Task.FromResult((data, (InboundRoutingDecision)TerminalActions.Unrecoverable)));

    var result = await RouteInboundPipelinesAsync<Services, Data, string, string, string, string, string, TestSession>(
      services, data, InboundPipelineTypes.Handling, routePipeline);

    result.ShouldBe(TerminalActions.Unrecoverable);
    routePipeline.Received(1).Invoke(services, data, InboundPipelineTypes.Handling, CancellationToken.None);
  }

  [TestMethod]
  public async Task route_inbound_pipelines__pipeline_decision__passes_updated_data_to_next_pipeline()
  {
    var services = CreateServices();
    var initialData = new Data();
    var updatedData = new Data();
    var routePipeline = Substitute.For<Func<Services, Data, InboundPipelineTypes, CancellationToken, Task<(Data, InboundRoutingDecision)>>>();
    routePipeline(services, initialData, InboundPipelineTypes.Capturing, CancellationToken.None)
      .Returns(Task.FromResult((updatedData, (InboundRoutingDecision)InboundPipelineTypes.Handling)));
    routePipeline(services, updatedData, InboundPipelineTypes.Handling, CancellationToken.None)
      .Returns(Task.FromResult((updatedData, (InboundRoutingDecision)TerminalActions.Exit)));

    var result = await RouteInboundPipelinesAsync<Services, Data, string, string, string, string, string, TestSession>(
      services, initialData, InboundPipelineTypes.Capturing, routePipeline);

    result.ShouldBe(TerminalActions.Exit);
    routePipeline.Received(1).Invoke(services, initialData, InboundPipelineTypes.Capturing, CancellationToken.None);
    routePipeline.Received(1).Invoke(services, updatedData, InboundPipelineTypes.Handling, CancellationToken.None);
  }

  [TestMethod]
  public async Task route_inbound_pipelines__cancelled_before_start__returns_exit_without_routing()
  {
    var services = CreateServices();
    var data = new Data();
    var routePipeline = Substitute.For<Func<Services, Data, InboundPipelineTypes, CancellationToken, Task<(Data, InboundRoutingDecision)>>>();
    using var cancellationSource = new CancellationTokenSource();
    await cancellationSource.CancelAsync();

    var result = await RouteInboundPipelinesAsync<Services, Data, string, string, string, string, string, TestSession>(
      services, data, InboundPipelineTypes.Capturing, routePipeline, cancellationSource.Token);

    result.ShouldBe(TerminalActions.Exit);
    routePipeline.DidNotReceive().Invoke(
      Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<InboundPipelineTypes>(), Arg.Any<CancellationToken>());
  }
}
