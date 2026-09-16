
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

readonly ref struct CapturingOperation<TServices, TData> {
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, CapturingStates, Exception?)>>? Capturing { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, VerifyingStates, Exception?)>>? Verifying { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, MappingStates, Exception?)>> Mapping { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ValidatingStates, Exception?)>> Validating { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, InsertingStates, Exception?)>> Inserting { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ConfirmingStates, Exception?)>> Confirming { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ConfirmingFinalStates, Exception?)>> ConfirmingFinal { get; init; }
}

partial class InboundFuncs
{
  internal static CapturingOperation<TServices, TData>
    GetCapturingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
      (CapturingActions action)
      where TServices : ICapturingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        CapturingActions.Capturing => new CapturingOperation<TServices, TData>{ Capturing = CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>},
        CapturingActions.Verifying => new CapturingOperation<TServices, TData>{ Verifying = VerifyEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>},
        CapturingActions.Mapping => new CapturingOperation<TServices, TData>{ Mapping = MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>},
        CapturingActions.Validating => new CapturingOperation<TServices, TData>{ Validating = ValidateInboxMessage<TServices, TData, TKey, TPayload>},
        CapturingActions.Inserting => new CapturingOperation<TServices, TData>{ Inserting = InsertInboxMessageAsync<TServices, TData, TKey, TPayload>},
        CapturingActions.Confirming => new CapturingOperation<TServices, TData>{ Confirming = ConfirmEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>},
        CapturingActions.ConfirmingFinal => new CapturingOperation<TServices, TData>{ ConfirmingFinal = ConfirmFinalEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>}
      };

  internal static ValueTask<(TData, CapturingSignal, Exception?)>
    ExecuteCapturingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      CapturingOperation<TServices, TData> operation,
      TServices services,
      TData data,
      CancellationToken ct = default
    )
  where TServices : ICapturingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    operation switch
    {
      { Capturing: not null } => operation.Capturing(services, data, ct).FromResult<TData, CapturingStates, CapturingSignal>(static states => states),
      { Verifying: not null } => operation.Verifying(services, data, ct).FromResult<TData, VerifyingStates, CapturingSignal>(static states => states),
      { Mapping: not null } => operation.Mapping(services, data, ct).FromResult<TData, MappingStates, CapturingSignal>(static states => states),
      { Validating: not null } => operation.Validating(services, data, ct).FromResult<TData, ValidatingStates, CapturingSignal>(static states => states),
      { Inserting: not null } => operation.Inserting(services, data, ct).FromResult<TData, InsertingStates, CapturingSignal>(static states => states),
      { Confirming: not null } => operation.Confirming(services, data, ct).FromResult<TData, ConfirmingStates, CapturingSignal>(static states => states),
      { ConfirmingFinal: not null } => operation.ConfirmingFinal(services, data, ct).FromResult<TData, ConfirmingFinalStates, CapturingSignal>(static states => states),
      _ => throw new InvalidOperationException($"invalid capturing operation.")
    };
}
