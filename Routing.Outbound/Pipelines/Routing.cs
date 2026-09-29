namespace Routing.Outbound;

partial class OutboundFuncs
{
  internal static Task<(object?[], string, string)> RoutePipelineAsync<TSession>(
    RoutingCapabilities<TSession> capabilities,
    object?[] data,
    string pipelineType,
    string? signal = default,
    CancellationToken ct = default)
  where TSession : ISessionService =>
    pipelineType switch
    {
      PipelineTypes.Persisting => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Persisting, CreatePersistingFunctions<TSession>(), data, signal ?? PersistingEntries.Start, ct),
      PipelineTypes.Publishing => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Publishing, CreatePublishingFunctions(), data, signal ?? PublishingEntries.Start, ct),
      PipelineTypes.Dispatching => RunPipelineAsync(capabilities.Running, capabilities.Pipeline.Dispatching, CreateDispatchingFunctions(), data, signal ?? DispatchingEntries.Start, ct),
      _ => ToTask((data, signal ?? pipelineType, TerminalActions.Unknown))
    };
}
