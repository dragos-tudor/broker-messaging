
namespace Pipelines.Inbound;

public interface IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession> :
  IDeadLetterServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
  where TSession : IDisposable;
