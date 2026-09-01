
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  public static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(string action)
      where TServices : IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
      where TData : IInboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TSession : IDisposable =>
      action.Split(".")[0] switch
      {
        EnvelopeActions.Scope => GetEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
        InboxActions.Scope => GetInboxOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(action),
        DeadLetterActions.Scope => GetDeadLetterOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
        EphemeralDeadLetterEnvelopeActions.Scope => GetEphemeralDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
        DeadLetterEnvelopeActions.Scope => GetDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
        _ => null
      };
}