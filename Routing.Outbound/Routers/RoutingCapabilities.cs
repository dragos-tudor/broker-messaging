namespace Routing.Outbound;

public sealed record RoutingCapabilities<TSession>
(
  RunningCapabilities Running,
  PipelineCapabilities<TSession> Pipeline
)
where TSession : ISessionService;

public sealed record RunningCapabilities
(
  GetPipelineConfig GetPipelineConfig,
  GetFastRetryOptions GetFastRetryOptions,
  IsFastRetryDelayedAsync IsFastRetryDelayedAsync,
  InstrumentPipeline InstrumentPipeline,
  InstrumentOperation InstrumentOperation
);
