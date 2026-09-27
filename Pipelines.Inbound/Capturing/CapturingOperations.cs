
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteCapturingOperationAsync<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) =>
    decision switch
    {
      CapturingActions.Capturing => CaptureEnvelope(capabilities.Capturing, data, ct),
      CapturingActions.Verifying => ToTask(VerifyEnvelope(capabilities.Verifying, data)),
      CapturingActions.Mapping => ToTask(MapEnvelope(capabilities.Mapping, data)),
      CapturingActions.Validating => ToTask(ValidateInboxMessage(capabilities.Validating, data)),
      CapturingActions.Inserting => InsertInboxMessageAsync(capabilities.Inserting, data, ct),
      CapturingActions.Confirming => ConfirmEnvelope(capabilities.Confirming, data, ct),
      CapturingActions.ConfirmingFinal => ConfirmFinalEnvelope(capabilities.Confirming, data, ct),
      _ => ToTask((data, CapturingEntries.End, default(Exception?)))
    };
}
