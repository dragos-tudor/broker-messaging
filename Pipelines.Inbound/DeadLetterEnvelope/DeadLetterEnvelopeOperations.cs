
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
      GetDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(DeadLetterEnvelopeOperation action)
  where TServices : IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    action switch
    {
      DeadLetterEnvelopeOperation.CheckingRetry => CheckRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      DeadLetterEnvelopeOperation.Redirecting => RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      DeadLetterEnvelopeOperation.Publishing => PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      DeadLetterEnvelopeOperation.Producing => ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      DeadLetterEnvelopeOperation.UpsertingRetry => UpsertRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      // Sending, Unrecoverable, Exit, Unknown all fall here — non-dispatchable, Router branches explicitly
      _ => default,
    };
}

internal enum DeadLetterEnvelopeOperation
{
  CheckingRetry,
  Deffering,
  Redirecting,
  Sending,                    // non-dispatchable — Router resolves to CheckingRetryForPublishing or ...ForProducing based on config
  Publishing,
  Producing,
  UpsertingRetry,
  Unrecoverable,
  Exit,
  Unknown
}
