
using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

readonly ref struct RedirectingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, Envelope.ConvertingStates, Exception?)>>? Converting { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetterEnvelope.RedirectingStates, Exception?)>>? Redirecting { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, Envelope.ConfirmingFinalStates, Exception?)>>? ConfirmingFinal { get; init; }
}

partial class InboundFuncs
{
  internal static RedirectingOperation<TServices, TData>
    GetRedirectingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(RedirectingActions action)
    where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
    where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => action switch
    {
      RedirectingActions.Converting => new RedirectingOperation<TServices, TData> { Converting = ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload> },
      RedirectingActions.Redirecting => new RedirectingOperation<TServices, TData> { Redirecting = RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation> },
      RedirectingActions.ConfirmingFinal => new RedirectingOperation<TServices, TData> { ConfirmingFinal = ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation> },
      _ => throw new InvalidOperationException($"Unknown redirecting action: {action}")
    };

  internal static ValueTask<(TData, RedirectingInput, Exception?)>
    ExecuteRedirectingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      RedirectingOperation<TServices, TData> operation, TServices services, TData data, CancellationToken ct = default)
    where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
    where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => operation switch
    {
      { Converting: not null } => operation.Converting(services, data, ct).FromResult<TData, Envelope.ConvertingStates, RedirectingInput>(static state => state),
      { Redirecting: not null } => operation.Redirecting(services, data, ct).FromResult<TData, DeadLetterEnvelope.RedirectingStates, RedirectingInput>(static state => state),
      { ConfirmingFinal: not null } => operation.ConfirmingFinal(services, data, ct).FromResult<TData, Envelope.ConfirmingFinalStates, RedirectingInput>(static state => state),
      _ => throw new InvalidOperationException("Unknown redirecting operation.")
    };
}
