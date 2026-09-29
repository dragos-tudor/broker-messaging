
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<(object?[], RoutingResult?)> RoutePipelinesAsync<TSession>(
    RoutingCapabilities<TSession> capabilities,
    RoutePipeline<TSession> routePipeline,
    object?[] data,
    string pipelineType,
    string? signal = default,
    CancellationToken ct = default)
  where TSession : ISessionService
  {
    while (!ct.IsCancellationRequested)
    {
      var (nextData, nextSignal, nextDecision) =
        await routePipeline(capabilities, data, pipelineType, signal, ct);

      if (IsTerminalAction(nextDecision))
        return (nextData, CreateRoutingResult(pipelineType, nextSignal, nextDecision));
      if (!IsPipelineType(nextDecision))
        return (nextData, CreateRoutingResult(pipelineType, nextSignal, nextDecision));

      data = nextData;
      pipelineType = nextDecision;
      signal = default;
    }
    return (data, default);
  }
}