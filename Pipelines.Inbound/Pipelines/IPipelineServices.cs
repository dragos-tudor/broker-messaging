
namespace Pipelines.Inbound;

public interface IPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession> :
  ICapturingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>,
  IHandlingServices<TKey, TPayload, TSession>,
  IDeadLetteringServices<TKey, TPayload>,
  IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TSession : IDisposable;
