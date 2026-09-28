using AutoFixture.AutoNSubstitute;

namespace Routing.Inbound;

partial class InboundTests
{
  static readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization
  {
    ConfigureMembers = true,
    GenerateDelegates = true
  });

  static object?[] CreateData() => new object?[6];

  static RunningCapabilities CreateRunningCapabilities(FastRetryOptions? options = null) => new(
    () => new PipelineConfig(),
    () => options ?? new FastRetryOptions { MaxRetryAttempts = 2, RetryBaseDelay = TimeSpan.Zero },
    (_, _, _) => Task.FromResult(true),
    (_, _) => { },
    (_, _, _, _) => { });

  static PipelineFunctions<string> CreateFunctions(
    AdvancePipeline advancePipeline,
    ExecuteOperationAsync<string> executeOperationAsync,
    PropagateException propagateException,
    CanFastRetry canFastRetry) => new(advancePipeline, executeOperationAsync, propagateException, canFastRetry);
}
