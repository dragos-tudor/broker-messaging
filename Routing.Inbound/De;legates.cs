
namespace Routing.Inbound;

public delegate void InstrumentPipeline(
  string signal,
  PipelineContext context);

public delegate void InstrumentOperation(
  string signal,
  string decision,
  PipelineContext context,
  Exception? exception);

public delegate Task<(object?[], string, string)> RoutePipeline<TSession>(
  RoutingCapabilities<TSession> capabilities,
  object?[] data,
  string pipelineType,
  string? initialSignal = default,
  CancellationToken ct = default)
where TSession : ISessionService;
