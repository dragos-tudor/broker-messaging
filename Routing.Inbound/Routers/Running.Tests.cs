#pragma warning disable CS4014

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
    var pipeline = CreatePipeline(InboundPipelineTypes.Capturing);
    var executeOperation = CreateExecuteOperation();
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, data, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((data, InboundPipelineTypes.Capturing));
    pipeline.Received(1).Invoke(TestSignal.Initial, Arg.Any<InboundPipelineConfig>());
    executeOperation.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>());
    propagateException.DidNotReceive().Invoke(Arg.Any<Data>(), Arg.Any<TestSignal>(), Arg.Any<Exception?>());
    services.Received(1).InstrumentPipeline(TestSignal.Initial, (object)InboundPipelineTypes.Capturing);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__terminal_action__returns_without_executing_operation()
  {
    var services = CreateServices();
    var data = CreateData();
    var pipeline = CreatePipeline(TerminalActions.Unrecoverable);
    var executeOperation = CreateExecuteOperation();
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, data, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((data, TerminalActions.Unrecoverable));
    executeOperation.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>());
    propagateException.DidNotReceive().Invoke(Arg.Any<Data>(), Arg.Any<TestSignal>(), Arg.Any<Exception?>());
  }

  [TestMethod]
  public async Task run_inbound_pipeline__operation_result__continues_with_updated_data_and_signal()
  {
    var services = CreateServices();
    var initialData = CreateData();
    var updatedData = CreateData();
    var operationDecision = new object();
    var pipeline = CreatePipeline(operationDecision, TerminalActions.Exit);
    var executeOperation = CreateExecuteOperation((updatedData, TestSignal.Completed, null));
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, initialData, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((updatedData, TerminalActions.Exit));
    executeOperation.Received(1).Invoke(operationDecision, services, initialData, CancellationToken.None);
    pipeline.Received(1).Invoke(TestSignal.Initial, Arg.Any<InboundPipelineConfig>());
    pipeline.Received(1).Invoke(TestSignal.Completed, Arg.Any<InboundPipelineConfig>());
    propagateException.Received(1).Invoke(updatedData, TestSignal.Completed, null);
    services.Received(1).InstrumentOperation(updatedData, TestSignal.Completed, null);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__retry_then_success__returns_final_operation_result()
  {
    var services = CreateServices(new FastRetryOptions { RetryBaseDelay = TimeSpan.Zero });
    var initialData = CreateData();
    var retriedData = CreateData();
    var completedData = CreateData();
    var operationDecision = new object();
    var pipeline = CreatePipeline(operationDecision, TerminalActions.Exit);
    var executeOperation = CreateExecuteOperation(
      (retriedData, TestSignal.Retry, null),
      (completedData, TestSignal.Completed, null));
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, initialData, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((completedData, TerminalActions.Exit));
    executeOperation.Received(1).Invoke(operationDecision, services, initialData, CancellationToken.None);
    executeOperation.Received(1).Invoke(operationDecision, services, retriedData, CancellationToken.None);
    propagateException.Received(1).Invoke(completedData, TestSignal.Completed, null);
    services.Received(1).InstrumentOperation(completedData, TestSignal.Completed, null);
    pipeline.Received(1).Invoke(TestSignal.Completed, Arg.Any<InboundPipelineConfig>());
  }

  [TestMethod]
  public async Task run_inbound_pipeline__operation_returns_exception__propagates_and_instruments_error()
  {
    var services = CreateServices();
    var initialData = CreateData();
    var updatedData = CreateData();
    var expectedException = new InvalidOperationException("operation failed");
    var operationDecision = new object();
    var pipeline = CreatePipeline(operationDecision, TerminalActions.Exit);
    var executeOperation = CreateExecuteOperation((updatedData, TestSignal.Completed, expectedException));
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, initialData, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry);

    result.ShouldBe((updatedData, TerminalActions.Exit));
    propagateException.Received(1).Invoke(updatedData, TestSignal.Completed, expectedException);
    services.Received(1).InstrumentOperation(updatedData, TestSignal.Completed, expectedException);
  }

  [TestMethod]
  public async Task run_inbound_pipeline__cancelled_before_start__returns_exit_without_callbacks()
  {
    var services = CreateServices();
    var data = CreateData();
    var pipeline = CreatePipeline();
    var executeOperation = CreateExecuteOperation();
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();
    using var cancellationSource = new CancellationTokenSource();
    await cancellationSource.CancelAsync();
    services.ClearReceivedCalls();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, data, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry, cancellationSource.Token);

    result.ShouldBe((data, TerminalActions.Exit));
    services.Received(1).GetInboundPipelineConfig();
    pipeline.DidNotReceive().Invoke(Arg.Any<TestSignal>(), Arg.Any<InboundPipelineConfig>());
    executeOperation.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>());
    services.DidNotReceive().InstrumentPipeline(Arg.Any<TestSignal>(), Arg.Any<object>());
  }

  [TestMethod]
  public async Task run_inbound_pipeline__cancelled_during_operation__returns_updated_data_and_exit()
  {
    var services = CreateServices();
    var initialData = CreateData();
    var updatedData = CreateData();
    var operationDecision = new object();
    var pipeline = CreatePipeline(operationDecision);
    using var cancellationSource = new CancellationTokenSource();
    var executeOperation = Substitute.For<Func<object, Services, Data, CancellationToken, Task<(Data, TestSignal, Exception?)>>>();
    executeOperation(operationDecision, services, initialData, cancellationSource.Token)
      .Returns(_ =>
      {
        cancellationSource.CancelAsync();
        return Task.FromResult((updatedData, TestSignal.Retry, (Exception?)null));
      });
    var propagateException = CreatePropagateException();
    var canFastRetry = CreateCanFastRetry();

    var result = await RunInboundPipelineAsync<Services, Data, string, string, string, string, byte[], IDisposable, TestSignal, object>(
      services, initialData, TestSignal.Initial, pipeline, executeOperation, propagateException, canFastRetry, cancellationSource.Token);

    result.ShouldBe((updatedData, TerminalActions.Exit));
    executeOperation.Received(1).Invoke(operationDecision, services, initialData, cancellationSource.Token);
    canFastRetry.DidNotReceive().Invoke(Arg.Any<TestSignal>());
    propagateException.Received(1).Invoke(updatedData, TestSignal.Retry, null);
    services.Received(1).InstrumentOperation(updatedData, TestSignal.Retry, null);
    pipeline.DidNotReceive().Invoke(TestSignal.Retry, Arg.Any<InboundPipelineConfig>());
  }
}
