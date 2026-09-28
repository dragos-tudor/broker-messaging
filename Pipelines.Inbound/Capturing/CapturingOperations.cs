
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteCapturingOperationAsync(
      CapturingCapabilities capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) =>
    decision switch
    {
      CapturingActions.Capturing => CaptureEnvelopeAsync(capabilities.Capturing, data, ct),
      CapturingActions.Verifying => ToTask(VerifyEnvelope(capabilities.Verifying, data)),
      CapturingActions.Mapping => ToTask(MapEnvelope(capabilities.Mapping, data)),
      CapturingActions.Validating => ToTask(ValidateInboxMessage(capabilities.Validating, data)),
      CapturingActions.Inserting => InsertInboxMessageAsync(capabilities.Inserting, data, ct),
      CapturingActions.Confirming => ConfirmEnvelopeAsync(capabilities.Confirming, data, ct),
      CapturingActions.ConfirmingFinal => ConfirmFinalEnvelopeAsync(capabilities.Confirming, data, ct),
      _ => ToTask((data, CapturingEntries.End, default(Exception?)))
    };
}
