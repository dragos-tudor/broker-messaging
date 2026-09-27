using Foundation.Extensions;
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

public sealed record PersistingCapabilities<TKey, TPayload, TSession>(
  ValidatingCapabilities<TKey, TPayload> Validating,
  TransactingCapabilities<TKey, TPayload, TSession> Transacting
)
where TSession : ISessionService;
