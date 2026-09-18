using Operations.Inbound.Envelope;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, RedirectingSignal, Exception?)>
    ExecuteRedirectingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      RedirectingTransition transition,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
    where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      transition switch
      {
        RedirectingActions.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data).FromResult<TData, ConvertingStates, RedirectingSignal>(static state => state),
        RedirectingActions.Redirecting => RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, RedirectingStates, RedirectingSignal>(static state => state),
        RedirectingActions.ConfirmingFinal => ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, ConfirmingFinalStates, RedirectingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
      };
}
