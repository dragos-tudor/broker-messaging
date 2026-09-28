
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, string)> RoutePipelineAsync<TSession>(
    RoutingCapabilities<TSession> capabilities,
    object?[] data,
    string pipelineType,
    string? signal = default,
    CancellationToken ct = default)
  where TSession : ISessionService =>
    pipelineType switch
    {
      PipelineTypes.Capturing => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Capturing, CreateCapturingFunctions(), data, signal ?? CapturingEntries.Start, ct),
      PipelineTypes.Redirecting => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Redirecting, CreateRedirectingFunctions(), data, signal ?? RedirectingEntries.Start, ct),
      PipelineTypes.Handling => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Handling, CreateHandlingFunctions<TSession>(), data, signal ?? HandlingEntries.Start, ct),
      PipelineTypes.DeadLettering => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.DeadLettering, CreateDeadLetteringFunctions(), data, signal ?? DeadLetteringEntries.Start, ct),
      PipelineTypes.Publishing => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Publishing, CreatePublishingFunctions(), data, signal ?? PublishingEntries.Start, ct),
      PipelineTypes.Dispatching => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Dispatching, CreateDispatchingFunctions(), data, signal ?? DispatchingEntries.Start, ct),
      _ => ToTask((data, signal ?? pipelineType, TerminalActions.Unknown))
    };

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