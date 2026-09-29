
namespace Pipelines.Outbound;

public record struct PipelineFunctions<TCapabilities>
(
  AdvancePipeline AdvancePipeline,
  ExecuteOperationAsync<TCapabilities> ExecuteOperationAsync,
  PropagateException PropagateException,
  CanFastRetry CanFastRetry
);