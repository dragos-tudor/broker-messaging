
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  public static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetInboundPipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(string action)
      where TServices : IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
      where TData : IInboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TSession : IDisposable =>
        GetEnvelopePipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetEphemeralDeadLetterEnvelopePipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetInboxPipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(action) ??
        GetDeadLetterPipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetDeadLetterEnvelopepipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action);
}