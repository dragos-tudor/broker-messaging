
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, InboundRoutingDecision)>
    RouteInboundPipelineAsync<
      TServices,
      TData,
      TKey,
      TValue,
      TMetadata,
      TConfirmation,
      TPayload,
      TSession>(
        TServices services,
        TData data,
        InboundPipelineTypes pipelineType,
        CancellationToken ct = default)
    where TServices : IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRoutingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable =>
      pipelineType switch
      {
        InboundPipelineTypes.Capturing =>
          RunCapturingPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
            services, data, ct),

        InboundPipelineTypes.Redirecting =>
          RunRedirectingPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
            services, data, ct),

        InboundPipelineTypes.Handling =>
          RunHandlingPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
            services, data, ct),

        InboundPipelineTypes.DeadLettering =>
          RunDeadLetteringPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
            services, data, ct),

        InboundPipelineTypes.Publishing =>
          RunPublishingPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
            services, data, ct),

        InboundPipelineTypes.Dispatching =>
          RunDispatchingPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
            services, data, ct),

        _ => throw new InvalidOperationException(
          $"Invalid inbound pipeline type {pipelineType}")
      };
}