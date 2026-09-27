using Foundation.Extensions;

namespace Pipelines.Outbound;

public sealed record OutboundPipelineCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
  PersistingCapabilities<TKey, TPayload, TSession> Persisting,
  PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Publishing,
  DispatchingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Dispatching
)
where TSession : ISessionService;
