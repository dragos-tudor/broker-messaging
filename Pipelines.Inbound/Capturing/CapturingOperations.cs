
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetCapturingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(string action)
      where TServices: ICapturingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TData: ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        CapturingActions.Capturing => CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        CapturingActions.Verifying => VerifyEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        CapturingActions.Mapping => MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
        CapturingActions.Validating => ValidateInboxMessage<TServices, TData, TKey, TPayload>,
        CapturingActions.Inserting => InsertInboxMessageAsync<TServices, TData, TKey, TPayload>,
        CapturingActions.Confirming => ConfirmEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        CapturingActions.ConfirmingFinal => ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
        _ => default,
      };
}