using Foundation.Extensions;

namespace Pipelines.Outbound;

public sealed record PipelineCapabilities<TSession>(
  PersistingCapabilities<TSession> Persisting,
  PublishingCapabilities Publishing,
  DispatchingCapabilities Dispatching
)
where TSession : ISessionService;
