
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<(TData, InboundRoutingDecision)>
    RunInboundPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession, TSignal, TDecision>(
      TServices services,
      TData data,
      TSignal signal,
      Func<TSignal, InboundPipelineConfig, TDecision> advancePipeline,
      Func<TDecision, TServices, TData, CancellationToken, Task<(TData, TSignal, Exception?)>> executeOperation,
      Func<TData, TSignal, Exception?, string?> propagateException,
      Func<TSignal, bool> canFastRetry,
      CancellationToken ct = default)
    where TServices : IInboundRoutingServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundRoutingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSignal: struct
    where TDecision: struct, IInboundDecision
    where TSession : IDisposable
  {
    var pipelineConfig = services.GetInboundPipelineConfig();
    while (!ct.IsCancellationRequested)
    {
      var decision = advancePipeline(signal, pipelineConfig);
      services.InstrumentPipeline(services, signal, decision);

      var pipelineType = decision.GetPipelineType();
      if (pipelineType != InboundPipelineTypes.None)
        return (data, pipelineType);

      var terminalAction = decision.GetTerminalAction();
      if (terminalAction != TerminalActions.None)
        return (data, terminalAction);

      var (nextData, nextSignal, exception) = await
        RunFastRetryAsync(services, data, decision, executeOperation, canFastRetry, DelayFastRetryAsync, ct);
      propagateException(nextData, nextSignal, exception);

      services.InstrumentOperation(services, nextData, nextSignal, exception);

      data = nextData;
      signal = nextSignal;
    }
    return (data, TerminalActions.Exit);
  }
}