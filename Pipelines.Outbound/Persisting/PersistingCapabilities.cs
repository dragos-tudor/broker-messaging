using Foundation.Extensions;
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

public sealed record PersistingCapabilities<TSession>(
  ValidatingCapabilities Validating,
  TransactingCapabilities<TSession> Transacting
)
where TSession : ISessionService;
