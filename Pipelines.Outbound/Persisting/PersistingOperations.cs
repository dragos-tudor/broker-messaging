using Foundation.Extensions;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecutePersistingOperationAsync<TSession>(
      PersistingCapabilities<TSession> capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default)
    where TSession : ISessionService => decision switch
  {
    PersistingActions.Validating => ToTask(ValidateOutboxMessage(capabilities.Validating, data)),
    PersistingActions.Transacting => TransactOutboxMessageAsync(capabilities.Transacting, data, ct),
    _ => throw new InvalidOperationException($"Invalid execute operation decision {decision}")
  };
}
