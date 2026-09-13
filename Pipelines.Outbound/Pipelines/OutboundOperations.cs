
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetOutboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession> (string action)
      where TServices: IPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
      where TData: IPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TSession: IDisposable =>
        GetPersistingOperation<TServices, TData, TKey, TPayload, TSession>(action) ??
        GetPublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action) ??
        GetDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action);
}