using AutoFixture.AutoNSubstitute;

namespace Routing.Inbound;

partial class IntegrationTests
{
  static RoutingCapabilities<ISessionService> CreateCapabilities(IFixture fixture) =>
    fixture.Create<RoutingCapabilities<ISessionService>>();

  static object?[] CreateData() => new object?[6];

  static IFixture CreateIntegrationFixture() => new Fixture().
    Customize(new AutoNSubstituteCustomization
    {
      ConfigureMembers = true,
      GenerateDelegates = true
    });

  static void ConfigureFastRetryCapabilities(IFixture fixture, FastRetryOptions options)
  {
    var getFastRetryOptions = fixture.Freeze<GetFastRetryOptions>();
    getFastRetryOptions().Returns(options);

    var isFastRetryDelayedAsync = fixture.Freeze<IsFastRetryDelayedAsync>();
    isFastRetryDelayedAsync(Arg.Any<int>(), Arg.Any<FastRetryOptions>(), Arg.Any<CancellationToken>())
      .Returns(call => ResiliencyFuncs.IsFastRetryDelayedAsync(
        call.ArgAt<int>(0),
        call.ArgAt<FastRetryOptions>(1),
        call.ArgAt<CancellationToken>(2)));
  }
}
