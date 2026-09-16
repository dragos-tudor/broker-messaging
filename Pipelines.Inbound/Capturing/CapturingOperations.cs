
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, CapturingSignal, Exception?)>
    ExecuteCapturingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      CapturingTransition transition,
      TServices services,
      TData data,
      CancellationToken ct = default
    )
    where TServices : ICapturingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      transition switch
      {
        CapturingActions.Capturing => CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, CapturingStates, CapturingSignal>(static states => states),
        CapturingActions.Verifying => VerifyEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, VerifyingStates, CapturingSignal>(static states => states),
        CapturingActions.Mapping => MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(services, data, ct).FromResult<TData, MappingStates, CapturingSignal>(static states => states),
        CapturingActions.Validating => ValidateInboxMessage<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ValidatingStates, CapturingSignal>(static states => states),
        CapturingActions.Inserting => InsertInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, InsertingStates, CapturingSignal>(static states => states),
        CapturingActions.Confirming => ConfirmEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, ConfirmingStates, CapturingSignal>(static states => states),
        CapturingActions.ConfirmingFinal => ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct).FromResult<TData, ConfirmingFinalStates, CapturingSignal>(static states => states),
        _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
      };
}
