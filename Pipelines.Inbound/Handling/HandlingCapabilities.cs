using Foundation.Extensions;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public sealed record HandlingCapabilities<TKey, TPayload, TSession>
(
  HandlingCapabilities<TKey, TPayload> Handling,
  TransactingCapabilities<TKey, TPayload, TSession> Transacting,
  SchedulingCapabilities<TKey, TPayload> Scheduling,
  AbandoningCapabilities<TKey, TPayload> Abandoning
)
where TSession: ISessionService;
