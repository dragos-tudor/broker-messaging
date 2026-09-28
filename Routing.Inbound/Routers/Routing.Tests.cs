namespace Routing.Inbound;

partial class InboundTests
{
  [TestMethod]
  public async Task route_pipelines__terminal_decision__returns_routing_result()
  {
    var capabilities = Fixture.Create<RoutingCapabilities<ISessionService>>();
    var data = CreateData();
    RoutePipeline<ISessionService> routePipeline = (_, currentData, pipelineType, signal, _) =>
      Task.FromResult((currentData, signal ?? "finished", TerminalActions.Unrecoverable));

    var (resultData, result) = await RoutePipelinesAsync(capabilities, routePipeline, data, PipelineTypes.Handling);

    resultData.ShouldBe(data);
    result.ShouldBe(new RoutingResult(PipelineTypes.Handling, "finished", TerminalActions.Unrecoverable));
  }

  [TestMethod]
  public async Task route_pipelines__pipeline_decision__passes_updated_data_to_next_pipeline()
  {
    var capabilities = Fixture.Create<RoutingCapabilities<ISessionService>>();
    var initialData = CreateData();
    var updatedData = CreateData();
    var routes = new List<(string Pipeline, object?[] Data)>();
    RoutePipeline<ISessionService> routePipeline = (_, data, pipelineType, _, _) =>
    {
      routes.Add((pipelineType, data));
      return Task.FromResult(pipelineType == PipelineTypes.Capturing
        ? (updatedData, "captured", PipelineTypes.Handling)
        : (updatedData, "handled", TerminalActions.Exit));
    };

    var (resultData, result) = await RoutePipelinesAsync(capabilities, routePipeline, initialData, PipelineTypes.Capturing);

    resultData.ShouldBe(updatedData);
    result.ShouldBe(new RoutingResult(PipelineTypes.Handling, "handled", TerminalActions.Exit));
    routes.ShouldBe([(PipelineTypes.Capturing, initialData), (PipelineTypes.Handling, updatedData)]);
  }

  [TestMethod]
  public async Task route_pipeline__unknown_pipeline__returns_unknown_terminal_decision()
  {
    var capabilities = Fixture.Create<RoutingCapabilities<ISessionService>>();
    var data = CreateData();

    var result = await RoutePipelineAsync(capabilities, data, "UnknownPipeline", "entry");

    result.ShouldBe((data, "entry", TerminalActions.Unknown));
  }

  [TestMethod]
  public async Task route_pipeline__known_pipeline_types__uses_each_canonical_entry()
  {
    var capabilities = Fixture.Create<RoutingCapabilities<ISessionService>>();
    var data = CreateData();
    using var cancellation = new CancellationTokenSource();
    await cancellation.CancelAsync();
    var routes = new (string Pipeline, string Entry)[]
    {
      (PipelineTypes.Capturing, CapturingEntries.Start),
      (PipelineTypes.Redirecting, RedirectingEntries.Start),
      (PipelineTypes.Handling, HandlingEntries.Start),
      (PipelineTypes.DeadLettering, DeadLetteringEntries.Start),
      (PipelineTypes.Publishing, PublishingEntries.Start),
      (PipelineTypes.Dispatching, DispatchingEntries.Start)
    };

    foreach (var (pipeline, entry) in routes)
    {
      var result = await RoutePipelineAsync(capabilities, data, pipeline, ct: cancellation.Token);
      result.ShouldBe((data, entry, TerminalActions.Exit));
    }
  }

  [TestMethod]
  public async Task route_pipelines__cancelled_before_start__returns_data_without_result()
  {
    var capabilities = Fixture.Create<RoutingCapabilities<ISessionService>>();
    var data = CreateData();
    var routePipeline = Substitute.For<RoutePipeline<ISessionService>>();
    using var cancellation = new CancellationTokenSource();
    await cancellation.CancelAsync();

    var (resultData, result) = await RoutePipelinesAsync(capabilities, routePipeline, data, PipelineTypes.Capturing, ct: cancellation.Token);

    resultData.ShouldBe(data);
    result.ShouldBeNull();
    routePipeline.DidNotReceive().Invoke(Arg.Any<RoutingCapabilities<ISessionService>>(), Arg.Any<object?[]>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
  }
}
