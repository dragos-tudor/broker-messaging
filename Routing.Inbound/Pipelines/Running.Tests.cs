namespace Routing.Inbound;

partial class InboundTests
{
  [TestMethod]
  public async Task run_pipeline__pipeline_decision__returns_pipeline_type_without_executing_operation()
  {
    var data = CreateData();
    var fixture = CreateFixture();
    var advance = Substitute.For<AdvancePipeline>();
    advance(TestSignals.Initial, Arg.Any<PipelineConfig>()).Returns(TestSignals.NextPipeline);
    var execute = Substitute.For<ExecuteOperationAsync<string>>();
    var propagate = Substitute.For<PropagateException>();
    var canRetry = Substitute.For<CanFastRetry>();
    var functions = CreateFunctions(advance, execute, propagate, canRetry);

    var result = await RunPipelineAsync(CreateRunningCapabilities(fixture), "capabilities", functions, data, TestSignals.Initial);

    result.ShouldBe((data, TestSignals.Initial, TestSignals.NextPipeline));
    execute.DidNotReceive().Invoke(Arg.Any<string>(), Arg.Any<object?[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task run_pipeline__terminal_decision__returns_without_executing_operation()
  {
    var data = CreateData();
    var fixture = CreateFixture();
    var advance = Substitute.For<AdvancePipeline>();
    advance(Arg.Any<string>(), Arg.Any<PipelineConfig>()).Returns(TerminalActions.Unrecoverable);
    var execute = Substitute.For<ExecuteOperationAsync<string>>();
    var functions = CreateFunctions(advance, execute, Substitute.For<PropagateException>(), Substitute.For<CanFastRetry>());

    var result = await RunPipelineAsync(CreateRunningCapabilities(fixture), "capabilities", functions, data, TestSignals.Initial);

    result.ShouldBe((data, TestSignals.Initial, TerminalActions.Unrecoverable));
    execute.DidNotReceive().Invoke(Arg.Any<string>(), Arg.Any<object?[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task run_pipeline__operation_result__continues_with_updated_data_and_signal()
  {
    var initialData = CreateData();
    var updatedData = CreateData();
    var fixture = CreateFixture();
    var advance = Substitute.For<AdvancePipeline>();
    advance(TestSignals.Initial, Arg.Any<PipelineConfig>()).Returns(TestSignals.Operation);
    advance(TestSignals.Completed, Arg.Any<PipelineConfig>()).Returns(TerminalActions.Exit);
    var execute = Substitute.For<ExecuteOperationAsync<string>>();
    execute("capabilities", initialData, TestSignals.Operation, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult((updatedData, TestSignals.Completed, (Exception?)null)));
    var propagate = Substitute.For<PropagateException>();
    var functions = CreateFunctions(advance, execute, propagate, Substitute.For<CanFastRetry>());

    var result = await RunPipelineAsync(CreateRunningCapabilities(fixture), "capabilities", functions, initialData, TestSignals.Initial);

    result.ShouldBe((updatedData, TestSignals.Completed, TerminalActions.Exit));
    propagate.Received(1).Invoke(updatedData, TestSignals.Completed, null);
  }

  [TestMethod]
  public async Task run_pipeline__fast_retry_then_success__returns_final_operation_result()
  {
    var initialData = CreateData();
    var retriedData = CreateData();
    var completedData = CreateData();
    var fixture = CreateFixture();
    var advance = Substitute.For<AdvancePipeline>();
    advance(TestSignals.Initial, Arg.Any<PipelineConfig>()).Returns(TestSignals.Operation);
    advance(TestSignals.Completed, Arg.Any<PipelineConfig>()).Returns(TerminalActions.Exit);
    var execute = Substitute.For<ExecuteOperationAsync<string>>();
    execute("capabilities", initialData, TestSignals.Operation, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult((retriedData, TestSignals.Retry, (Exception?)null)));
    execute("capabilities", retriedData, TestSignals.Operation, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult((completedData, TestSignals.Completed, (Exception?)null)));
    var canRetry = Substitute.For<CanFastRetry>();
    canRetry(TestSignals.Retry).Returns(true);
    var delayedRetry = Substitute.For<IsFastRetryDelayedAsync>();
    delayedRetry(Arg.Any<int>(), Arg.Any<FastRetryOptions>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(true), Task.FromResult(false));
    var running = CreateRunningCapabilities(fixture) with { IsFastRetryDelayedAsync = delayedRetry };
    var functions = CreateFunctions(advance, execute, Substitute.For<PropagateException>(), canRetry);

    var result = await RunPipelineAsync(running, "capabilities", functions, initialData, TestSignals.Initial);

    result.ShouldBe((completedData, TestSignals.Completed, TerminalActions.Exit));
    execute.Received(2).Invoke("capabilities", Arg.Any<object?[]>(), TestSignals.Operation, Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task run_pipeline__operation_exception__propagates_and_instruments_error()
  {
    var data = CreateData();
    var fixture = CreateFixture();
    var exception = new InvalidOperationException("failed");
    var advance = Substitute.For<AdvancePipeline>();
    advance(TestSignals.Initial, Arg.Any<PipelineConfig>()).Returns(TestSignals.Operation);
    advance(TestSignals.Completed, Arg.Any<PipelineConfig>()).Returns(TerminalActions.Exit);
    var execute = Substitute.For<ExecuteOperationAsync<string>>();
    execute(Arg.Any<string>(), Arg.Any<object?[]>(), TestSignals.Operation, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult((data, TestSignals.Completed, (Exception?)exception)));
    var propagate = Substitute.For<PropagateException>();
    var instrumentOperation = Substitute.For<InstrumentOperation>();
    var running = CreateRunningCapabilities(fixture) with { InstrumentOperation = instrumentOperation };
    var functions = CreateFunctions(advance, execute, propagate, Substitute.For<CanFastRetry>());

    await RunPipelineAsync(running, "capabilities", functions, data, TestSignals.Initial);

    propagate.Received(1).Invoke(data, TestSignals.Completed, exception);
    instrumentOperation.Received(1).Invoke(TestSignals.Completed, Arg.Any<PipelineContext>(), exception);
  }

  [TestMethod]
  public async Task run_pipeline__cancelled_before_start__returns_without_advancing()
  {
    var data = CreateData();
    var fixture = CreateFixture();
    var advance = Substitute.For<AdvancePipeline>();
    using var cancellation = new CancellationTokenSource();
    await cancellation.CancelAsync();
    var functions = CreateFunctions(advance, Substitute.For<ExecuteOperationAsync<string>>(), Substitute.For<PropagateException>(), Substitute.For<CanFastRetry>());

    var result = await RunPipelineAsync(CreateRunningCapabilities(fixture), "capabilities", functions, data, TestSignals.Initial, cancellation.Token);

    result.ShouldBe((data, TestSignals.Initial, TerminalActions.Exit));
    advance.DidNotReceive().Invoke(Arg.Any<string>(), Arg.Any<PipelineConfig>());
  }
}
