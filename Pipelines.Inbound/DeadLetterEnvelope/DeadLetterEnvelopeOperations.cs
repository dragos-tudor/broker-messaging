
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
      GetEphemeralDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(string action)
  where TServices : IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    action switch
    {
      EphemeralDeadLetterEnvelopeActions.Redirecting => RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      EphemeralDeadLetterEnvelopeActions.CheckingRetry => CheckRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      EphemeralDeadLetterEnvelopeActions.UpsertingRetry => UpsertRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      // Sending, Unrecoverable, Exit, Unknown all fall here — non-dispatchable, Router branches explicitly
      _ => default,
    };

  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
      GetDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(string action)
  where TServices : IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    action switch
    {
      DeadLetterEnvelopeActions.Publishing => PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      DeadLetterEnvelopeActions.Producing => ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      // Sending, Unrecoverable, Exit, Unknown all fall here — non-dispatchable, Router branches explicitly
      _ => default,
    };
}
