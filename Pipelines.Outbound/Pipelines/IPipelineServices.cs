
namespace Pipelines.Outbound;

public interface IPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession> :
  IPersistingServices<TKey, TPayload, TSession>,
  IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TSession : IDisposable;
