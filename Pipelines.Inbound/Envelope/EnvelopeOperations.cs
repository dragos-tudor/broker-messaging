
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(string action)
      where TServices: IEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TData: IEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        EnvelopeActions.Capturing => CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        EnvelopeActions.Validating => ValidateEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        EnvelopeActions.Mapping => MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
        EnvelopeActions.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        EnvelopeActions.Confirming => ConfirmEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        _ => default,
      };
}