using Foundation.Extensions;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteHandlingOperationAsync<TSession>(
      HandlingCapabilities<TSession> capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default)
    where TSession : ISessionService =>
    decision switch
    {
      HandlingActions.Handling => HandleInboxMessageAsync(capabilities.Handling, data, ct),
      HandlingActions.Transacting => TransactInboxMessageAsync(capabilities.Transacting, data, ct),
      HandlingActions.Scheduling => ScheduleInboxMessageAsync(capabilities.Scheduling, data, ct),
      HandlingActions.Abandoning => AbandonInboxMessageAsync(capabilities.Abandoning, data, ct),
      _ => ToTask((data, HandlingEntries.End, default(Exception?)))
    };
}
