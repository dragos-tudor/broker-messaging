
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<TerminalActions>
    RouteInboundPipelinesAsync<
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
        Func<
          TServices,
          TData,
          InboundPipelineTypes,
          CancellationToken,
          Task<(TData, InboundRoutingDecision)>> routePipeline,
        CancellationToken ct = default)
    where TServices : IInboundRunningServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRunningData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    while (!ct.IsCancellationRequested)
    {
      var (nextData, decision) =
        await routePipeline(services, data, pipelineType, ct);

      if (decision is TerminalActions terminalAction)
        return terminalAction;

      if (decision is InboundPipelineTypes nextPipelineType)
        pipelineType = nextPipelineType;

      data = nextData;
    }

    return TerminalActions.Exit;
  }
}