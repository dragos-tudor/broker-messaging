
using Foundation.Extensions;

namespace Pipelines.Inbound;

public sealed record PipelineCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
(
  CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Capturing,
  RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Redirecting,
  HandlingCapabilities<TKey, TPayload, TSession> Handling,
  DeadLetteringCapabilities<TKey, TPayload> DeadLettering,
  PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Publishing,
  DispatchingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Dispatching
)
where TSession : ISessionService;
