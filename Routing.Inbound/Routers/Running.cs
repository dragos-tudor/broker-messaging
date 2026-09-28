
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<(object?[], string, string)> RunPipelineAsync<TCapabilities>(
    RunningCapabilities capabilities,
    TCapabilities pipelineCapabilities,
    PipelineFunctions<TCapabilities> functions,
    object?[] data,
    string signal,
    CancellationToken ct = default)
  {
    var options = capabilities.GetFastRetryOptions();
    var config = capabilities.GetPipelineConfig();
    var decision = TerminalActions.Exit;

    while (!ct.IsCancellationRequested)
    {
      decision = functions.AdvancePipeline(signal, config);
      capabilities.InstrumentPipeline(signal, decision, CreatePipelineContext(data));

      if (IsPipelineType(decision)) return (data, signal, decision);
      if (IsTerminalAction(decision)) return (data, signal, decision);

      var (nextData, nextSignal, exception) = await functions.
        ExecuteOperationAsync(pipelineCapabilities, data, decision, ct);

      var retryCount = 0;
      while(functions.CanFastRetry(nextSignal) && await capabilities.
            IsFastRetryDelayedAsync(retryCount++, options, ct))
        (nextData, nextSignal, exception) = await functions.
          ExecuteOperationAsync(pipelineCapabilities, nextData, decision, ct);

      functions.PropagateException(nextData, nextSignal, exception);
      capabilities.InstrumentOperation(nextSignal, CreatePipelineContext(nextData), exception);

      data = nextData;
      signal = nextSignal;
    }
    return (data, signal, decision);
  }
}