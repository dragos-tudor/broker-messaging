
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task
    RunInboundPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
      TServices services,
      TData data,
      string action,
      string pipeline,
      CancellationToken ct = default)
    where TServices : IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : InboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    while (true)
    {

    }
  }

}