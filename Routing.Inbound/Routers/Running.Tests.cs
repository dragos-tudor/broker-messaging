
using Services = Routing.Inbound.IInboundRoutingServices<string, string, string, string, byte[], System.IDisposable>;
using Data = Routing.Inbound.IInboundRoutingData<string, string, string, string, byte[]>;

namespace Routing.Inbound;

partial class InboundTests
{
  [TestMethod]
  public async Task run_inbound_pipeline__pipeline_decision__returns_pipeline_type_without_executing_operation()
  {
    var services = CreateServices();
    var data = CreateData();
    var advancePipeline = CreateAdvancePipeline(InboundPipelineTypes.Capturing);
    var executeOperation = CreateExecuteOperation();
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, data, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((data, InboundPipelineTypes.Capturing));
    advancePipeline.Received(1).Invoke(TestStates.Initial, Arg.Any<InboundPipelineConfig>());
    executeOperation.DidNotReceive().Invoke(Arg.Any<TestDecision>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>());
    propagateException.DidNotReceive().Invoke(Arg.Any<Data>(), Arg.Any<TestSignal>(), Arg.Any<Exception?>());
    services.Received(1).InstrumentPipeline(services, (TestSignal)TestStates.Initial, (TestDecision)InboundPipelineTypes.Capturing);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__terminal_action__returns_without_executing_operation()
  {
    var services = CreateServices();
    var data = CreateData();
    var advancePipeline = CreateAdvancePipeline(TerminalActions.Unrecoverable);
    var executeOperation = CreateExecuteOperation();
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, data, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((data, TerminalActions.Unrecoverable));
    executeOperation.DidNotReceive().Invoke(Arg.Any<TestDecision>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>());
    propagateException.DidNotReceive().Invoke(Arg.Any<Data>(), Arg.Any<TestSignal>(), Arg.Any<Exception?>());
  }

  [TestMethod]
  public async Task run_inbound_pipeline__operation_result__continues_with_updated_data_and_signal()
  {
    var services = CreateServices();
    var initialData = CreateData();
    var updatedData = CreateData();
    var operationDecision = TerminalActions.None;
    var advancePipeline = CreateAdvancePipeline(operationDecision, TerminalActions.Exit);
    var executeOperation = CreateExecuteOperation((updatedData, TestStates.Completed, null));
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, initialData, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((updatedData, TerminalActions.Exit));
    advancePipeline.Received(1).Invoke(TestStates.Initial, Arg.Any<InboundPipelineConfig>());
    executeOperation.Received(1).Invoke(operationDecision, services, initialData, CancellationToken.None);
    propagateException.Received(1).Invoke(updatedData, TestStates.Completed, null);
    advancePipeline.Received(1).Invoke(TestStates.Completed, Arg.Any<InboundPipelineConfig>());
    services.Received(1).InstrumentOperation(services, updatedData, (TestSignal)TestStates.Completed, null);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__retry_then_success__returns_final_operation_result()
  {
    var services = CreateServices(new FastRetryOptions { RetryBaseDelay = TimeSpan.Zero });
    var initialData = CreateData();
    var retriedData = CreateData();
    var completedData = CreateData();
    var operationDecision = TerminalActions.None;
    var advancePipeline = CreateAdvancePipeline(operationDecision, TerminalActions.Exit);
    var executeOperation = CreateExecuteOperation(
      (retriedData, TestStates.Retry, null),
      (completedData, TestStates.Completed, null));
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, initialData, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((completedData, TerminalActions.Exit));
    advancePipeline.Received(1).Invoke(TestStates.Completed, Arg.Any<InboundPipelineConfig>());
    executeOperation.Received(1).Invoke(operationDecision, services, initialData, CancellationToken.None);
    executeOperation.Received(1).Invoke(operationDecision, services, retriedData, CancellationToken.None);
    propagateException.Received(1).Invoke(completedData, TestStates.Completed, null);
    services.Received(1).InstrumentOperation(services, completedData, (TestSignal)TestStates.Completed, null);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__operation_returns_exception__propagates_and_instruments_error()
  {
    var services = CreateServices();
    var initialData = CreateData();
    var updatedData = CreateData();
    var expectedException = new InvalidOperationException("operation failed");
    var operationDecision = TerminalActions.None;
    var advancePipeline = CreateAdvancePipeline(operationDecision, TerminalActions.Exit);
    var executeOperation = CreateExecuteOperation((updatedData, TestStates.Completed, expectedException));
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, initialData, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((updatedData, TerminalActions.Exit));
    propagateException.Received(1).Invoke(updatedData, TestStates.Completed, expectedException);
    services.Received(1).InstrumentOperation(services, updatedData, (TestSignal)TestStates.Completed, expectedException);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__cancelled_before_start__returns_exit_without_callbacks()
  {
    var services = CreateServices();
    var data = CreateData();
    var advancePipeline = CreateAdvancePipeline();
    var executeOperation = CreateExecuteOperation();
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();
    using var cancellationSource = new CancellationTokenSource();
    await cancellationSource.CancelAsync();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, data, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry, cancellationSource.Token);

    result.ShouldBe((data, TerminalActions.Exit));
    services.Received(1).GetInboundPipelineConfig();
    advancePipeline.DidNotReceive().Invoke(Arg.Any<TestSignal>(), Arg.Any<InboundPipelineConfig>());
    executeOperation.DidNotReceive().Invoke(Arg.Any<TestDecision>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>());
    services.DidNotReceive().InstrumentPipeline(services, Arg.Any<TestSignal>(), Arg.Any<TestDecision>());
  }

  [TestMethod]
  public async Task run_inbound_pipeline__cancelled_during_operation__returns_updated_data_and_exit()
  {
    var services = CreateServices();
    var initialData = CreateData();
    var updatedData = CreateData();
    var operationDecision = TerminalActions.None;
    var advancePipeline = CreateAdvancePipeline(operationDecision);
    using var cancellationSource = new CancellationTokenSource();
    var executeOperation = Substitute.For<Func<TestDecision, Services, Data, CancellationToken, Task<(Data, TestSignal, Exception?)>>>();
    executeOperation(operationDecision, services, initialData, cancellationSource.Token)
      .Returns(_ =>
      {
        cancellationSource.CancelAsync();
        return Task.FromResult((updatedData, (TestSignal)TestStates.Retry, (Exception?)null));
      });
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, TestDecision>(
      services, initialData, TestStates.Initial, advancePipeline, executeOperation, propagateException, canFastRetry, cancellationSource.Token);

    result.ShouldBe((updatedData, TerminalActions.Exit));
    advancePipeline.DidNotReceive().Invoke(TestStates.Retry, Arg.Any<InboundPipelineConfig>());
    executeOperation.Received(1).Invoke(operationDecision, services, initialData, cancellationSource.Token);
    propagateException.Received(1).Invoke(updatedData, TestStates.Retry, null);
    canFastRetry.DidNotReceive().Invoke(Arg.Any<TestSignal>());
    services.Received(1).InstrumentOperation(services, updatedData, (TestSignal)TestStates.Retry, null);
  }
}
