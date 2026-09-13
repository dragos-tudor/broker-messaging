
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession> (string action)
      where TServices: IPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
      where TData: IPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TSession: IDisposable =>
        GetCapturingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetRedirectingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetHandlingOperation<TServices, TData, TKey, TPayload, TSession>(action) ??
        GetDeadLetteringOperation<TServices, TData, TKey, TPayload>(action) ??
        GetPublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action);
}