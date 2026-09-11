
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetRedirectingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(string action)
      where TServices: IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
      where TData: IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        RedirectingActions.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
        RedirectingActions.Redirecting => RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        RedirectingActions.ConfirmingFinal => ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        _ => default,
      };
}