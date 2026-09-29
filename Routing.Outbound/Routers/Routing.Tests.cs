namespace Routing.Outbound;

partial class OutboundTests
{
  [TestMethod]
  public async Task route_pipelines__terminal_decision__returns_routing_result()
  {
    var fixture = CreateFixture();
    var capabilities = fixture.Create<RoutingCapabilities<ISessionService>>();
    var data = CreateData();
    RoutePipeline<ISessionService> routePipeline = (_, currentData, pipelineType, signal, _) =>
      Task.FromResult((currentData, signal ?? "finished", TerminalActions.Unrecoverable));

    var (resultData, result) = await RoutePipelinesAsync(capabilities, routePipeline, data, PipelineTypes.Publishing);

    resultData.ShouldBe(data);
    result.ShouldBe(new RoutingResult(PipelineTypes.Publishing, "finished", TerminalActions.Unrecoverable));
  }

  [TestMethod]
  public async Task route_pipelines__pipeline_decision__passes_updated_data_to_next_pipeline()
  {
    var fixture = CreateFixture();
    var capabilities = fixture.Create<RoutingCapabilities<ISessionService>>();
    var initialData = CreateData();
    var updatedData = CreateData();
    var routes = new List<(string Pipeline, object?[] Data)>();
    RoutePipeline<ISessionService> routePipeline = (_, data, pipelineType, _, _) =>
    {
      routes.Add((pipelineType, data));
      return Task.FromResult(pipelineType == PipelineTypes.Persisting
        ? (updatedData, "persisted", PipelineTypes.Publishing)
        : (updatedData, "published", TerminalActions.Exit));
    };

    var (resultData, result) = await RoutePipelinesAsync(capabilities, routePipeline, initialData, PipelineTypes.Persisting);

    resultData.ShouldBe(updatedData);
    result.ShouldBe(new RoutingResult(PipelineTypes.Publishing, "published", TerminalActions.Exit));
    routes.ShouldBe([(PipelineTypes.Persisting, initialData), (PipelineTypes.Publishing, updatedData)]);
  }

  [TestMethod]
  public async Task route_pipelines__cancelled_before_start__returns_data_without_result()
  {
    var fixture = CreateFixture();
    var capabilities = fixture.Create<RoutingCapabilities<ISessionService>>();
    var data = CreateData();
    var routePipeline = Substitute.For<RoutePipeline<ISessionService>>();
    using var cancellation = new CancellationTokenSource();
    await cancellation.CancelAsync();

    var (resultData, result) = await RoutePipelinesAsync(capabilities, routePipeline, data, PipelineTypes.Persisting, ct: cancellation.Token);

    resultData.ShouldBe(data);
    result.ShouldBeNull();
    routePipeline.DidNotReceive().Invoke(Arg.Any<RoutingCapabilities<ISessionService>>(), Arg.Any<object?[]>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
  }
}
