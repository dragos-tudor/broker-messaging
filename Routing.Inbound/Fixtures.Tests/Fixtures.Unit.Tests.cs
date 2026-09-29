
using AutoFixture.AutoNSubstitute;

namespace Routing.Inbound;

internal static class TestSignals
{
  internal const string Initial = "Initial";
  internal const string Retry = "Retry";
  internal const string Completed = "Completed";
  internal const string Operation = "Operation";
  internal const string NextPipeline = "PipelineTypes.Capturing";
}

partial class InboundTests
{
  static object?[] CreateData() => new object?[6];

  static FastRetryOptions CreateFastRetryOptions() =>
    new()
    {
      MaxRetryAttempts = 2,
      RetryBaseDelay = TimeSpan.Zero
    };

  static IFixture CreateFixture() => new Fixture()
    .Customize(new AutoNSubstituteCustomization
    {
      ConfigureMembers = true,
      GenerateDelegates = true
    });

  static RunningCapabilities CreateRunningCapabilities(
    IFixture fixture,
    FastRetryOptions? options = null)
  {
    options = options ?? CreateFastRetryOptions();
    fixture.Inject(options);

    var capabilities = fixture.Create<RunningCapabilities>();
    capabilities
      .IsFastRetryDelayedAsync(Arg.Any<int>(),options, Arg.Any<CancellationToken>())
      .Returns(true);

    return capabilities;
  }

  static PipelineFunctions<string> CreateFunctions(
    AdvancePipeline advancePipeline,
    ExecuteOperationAsync<string> executeOperationAsync,
    PropagateException propagateException,
    CanFastRetry canFastRetry) =>
      new(
        advancePipeline,
        executeOperationAsync,
        propagateException,
        canFastRetry
      );
}
