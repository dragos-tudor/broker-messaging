
namespace Pipelines.Outbound;

public delegate string AdvancePipeline(
  string signal,
  PipelineConfig pipelineConfig);

public delegate bool CanFastRetry(string signal);

public delegate PipelineConfig GetPipelineConfig();

public delegate Task<(object?[], string, Exception?)> ExecuteOperationAsync<TCapabilities>(
  TCapabilities capabilities,
  object?[] data,
  string decision,
  CancellationToken ct = default
);

public delegate string? PropagateException(
  object?[] data,
  string signal,
  Exception? exception);