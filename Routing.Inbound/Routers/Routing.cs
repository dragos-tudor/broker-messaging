
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<TerminalActions>
    RouteInboundPipelinesAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
      TServices services,
      TData data,
      InboundPipelineTypes currentPipelineType,
      Func<TServices, TData, InboundPipelineTypes, CancellationToken, Task<(TData, InboundRoutingDecision)>> routePipeline,
      CancellationToken ct = default)
    where TServices : IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRoutingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    while (!ct.IsCancellationRequested)
    {
      var (nextData, decision) =
        await routePipeline(services, data, currentPipelineType, ct);

      var terminalAction = decision.GetTerminalAction();
      if (terminalAction != TerminalActions.None)
        return terminalAction;

      var pipelineType = decision.GetPipelineType();
      if (pipelineType != InboundPipelineTypes.None)
        currentPipelineType = pipelineType;

      data = nextData;
    }

    return TerminalActions.Exit;
  }
}