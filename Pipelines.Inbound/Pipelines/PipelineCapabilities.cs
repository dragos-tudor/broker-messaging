
namespace Pipelines.Inbound;

public sealed record PipelineCapabilities<TSession>
(
  CapturingCapabilities Capturing,
  RedirectingCapabilities Redirecting,
  HandlingCapabilities<TSession> Handling,
  DeadLetteringCapabilities DeadLettering,
  PublishingCapabilities Publishing,
  DispatchingCapabilities Dispatching
)
where TSession: ISessionService;