using Operations.Inbound.Envelope;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, RedirectingSignal, Exception?)>
    ExecuteRedirectingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      RedirectingDecision decision,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
    where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      decision switch
      {
        RedirectingActions.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data).FromResult<TData, ConvertingStates, RedirectingSignal>(static state => state),
        RedirectingActions.Redirecting => RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, RedirectingStates, RedirectingSignal>(static state => state),
        RedirectingActions.ConfirmingFinal => ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, ConfirmingFinalStates, RedirectingSignal>(static state => state),
        _ => ToResult<TData, RedirectingSignal>(data, RedirectingEntries.End)
      };
}
