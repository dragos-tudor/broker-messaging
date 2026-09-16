using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, RedirectingSignal, Exception?)>
    ExecuteRedirectingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(RedirectingActions action, TServices services, TData data, CancellationToken ct = default)
    where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
    where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        RedirectingActions.Converting => ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, Envelope.ConvertingStates, RedirectingSignal>(static state => state),
        RedirectingActions.Redirecting => RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, DeadLetterEnvelope.RedirectingStates, RedirectingSignal>(static state => state),
        RedirectingActions.ConfirmingFinal => ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, Envelope.ConfirmingFinalStates, RedirectingSignal>(static state => state),
      };
}
