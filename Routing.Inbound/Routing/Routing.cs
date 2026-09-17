
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<TerminalActions>
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
        Func<
          TServices,
          TData,
          InboundPipelineTypes,
          CancellationToken,
          Task<(TData, InboundRoutingTransition)>> pipelineExecutor,
        CancellationToken ct = default)
    where TServices : IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRunningData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    while (!ct.IsCancellationRequested)
    {
      var (nextData, transition) =
        await pipelineExecutor(services, data, pipelineType, ct);

      if (transition is TerminalActions terminalAction)
        return terminalAction;

      if (transition is InboundPipelineTypes nextPipelineType)
        pipelineType = nextPipelineType;

      data = nextData;
    }

    return TerminalActions.Exit;
  }
}