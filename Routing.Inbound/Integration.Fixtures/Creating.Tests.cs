namespace Routing.Inbound;

partial class IntegrationTests
{
  static RoutingCapabilities<ISessionService> CreateCapabilities(IFixture fixture) =>
    fixture.Create<RoutingCapabilities<ISessionService>>();

  static object?[] CreateData() => new object?[6];
}
