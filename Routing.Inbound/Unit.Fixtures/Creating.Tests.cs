using Services = Routing.Inbound.IInboundRoutingServices<string, string, string, string, byte[], System.IDisposable>;
using Data = Routing.Inbound.IInboundRoutingData<string, string, string, string, byte[]>;

namespace Routing.Inbound;

partial class InboundTests
{
  static Data CreateData() => Substitute.For<Data>();

  static Services CreateServices(FastRetryOptions? options = null)
  {
    var services = Substitute.For<Services>();
    services.GetInboundPipelineConfig().Returns(new InboundPipelineConfig());
    services.GetFastRetryOptions().Returns(options ?? new FastRetryOptions());
    return services;
  }

  static Func<TestSignal, InboundPipelineConfig, TestDecision> CreateAdvancePipeline(params TestDecision[] decisions)
  {
    var advancePipeline = Substitute.For<Func<TestSignal, InboundPipelineConfig, TestDecision>>();
    var decisionIndex = 0;
    advancePipeline(Arg.Any<TestSignal>(), Arg.Any<InboundPipelineConfig>())
      .Returns(_ => decisions[decisionIndex++]);
    return advancePipeline;
  }

  static Func<TestDecision, Services, Data, CancellationToken, Task<(Data, TestSignal, Exception?)>>
    CreateExecuteOperation(params (Data, TestSignal, Exception?)[] results)
  {
    var executeOperation = Substitute.For<Func<TestDecision, Services, Data, CancellationToken, Task<(Data, TestSignal, Exception?)>>>();
    var resultIndex = 0;
    executeOperation(Arg.Any<TestDecision>(), Arg.Any<Services>(), Arg.Any<Data>(), Arg.Any<CancellationToken>())
      .Returns(_ => Task.FromResult(results[resultIndex++]));
    return executeOperation;
  }

  static Func<Data, TestSignal, Exception?, string?> CreatePropagateException()
  {
    var propagateException = Substitute.For<Func<Data, TestSignal, Exception?, string?>>();
    propagateException(Arg.Any<Data>(), Arg.Any<TestSignal>(), Arg.Any<Exception?>()).Returns("propagated");
    return propagateException;
  }

  static Func<TestSignal, bool> CreateCanFastRetry()
  {
    var canFastRetry = Substitute.For<Func<TestSignal, bool>>();
    canFastRetry(Arg.Any<TestSignal>()).Returns(false);
    canFastRetry(TestStates.Retry).Returns(true);
    return canFastRetry;
  }
}
