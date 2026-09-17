
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<(TData, InboundRoutingTransition)>
    RunInboundPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession, TSignal, TTransition>(
      TServices services,
      TData data,
      TSignal signal,
      Func<TSignal, InboundPipelineConfig, TTransition> pipeline,
      Func<TTransition, TServices, TData, CancellationToken, ValueTask<(TData, TSignal, Exception?)>> operationExecutor,
      CancellationToken ct = default)
    where TServices : IInboundRunningServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRunningData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    var pipelineConfig = services.GetInboundPipelineConfig();
    while (!ct.IsCancellationRequested)
    {
      var transition = pipeline(signal, pipelineConfig);
      services.InstrumentPipeline(signal, transition);

      if (transition is InboundPipelineTypes pipelineType)
        return (data, pipelineType);
      if (transition is TerminalActions terminalAction)
        return (data, terminalAction);

      var (nextData, nextSignal, exception) = await operationExecutor(transition, services, data, ct);
      services.InstrumentOperation(nextData, nextSignal, exception);

      data = nextData;
      signal = nextSignal;
    }
    return (data, TerminalActions.Exit);
  }
}