
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteRedirectingOperationAsync(
      RedirectingCapabilities capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) =>
    decision switch
    {
      RedirectingActions.Converting => ToTask(ConvertEnvelope(capabilities.Converting, data)),
      RedirectingActions.Redirecting => RedirectDeadLetterEnvelopeAsync(capabilities.Redirecting, data, ct),
      RedirectingActions.ConfirmingFinal => ConfirmFinalEnvelopeAsync(capabilities.Confirming, data, ct),
      _ => ToTask((data, RedirectingEntries.End, default(Exception?)))
    };
}
