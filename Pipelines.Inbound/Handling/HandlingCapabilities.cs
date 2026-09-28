using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public sealed record HandlingCapabilities<TSession>
(
  HandlingCapabilities Handling,
  TransactingCapabilities<TSession> Transacting,
  SchedulingCapabilities Scheduling,
  AbandoningCapabilities Abandoning
)
where TSession: ISessionService;
