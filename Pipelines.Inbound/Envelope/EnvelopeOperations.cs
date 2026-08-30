
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>? GetEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(EnvelopeOperation action)
    where TServices: IEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData: IEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    =>
    action switch
    {
      EnvelopeOperation.Capturing => CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      EnvelopeOperation.Validating => ValidateEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      EnvelopeOperation.Mapping => MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      EnvelopeOperation.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      EnvelopeOperation.Confirming => ConfirmEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      _ => default,
    };
}

internal enum EnvelopeOperation
{
  Capturing,
  Validating,
  Mapping,
  Converting,
  Confirming,
  Unrecoverable,
  Exit,
  Unknown
}