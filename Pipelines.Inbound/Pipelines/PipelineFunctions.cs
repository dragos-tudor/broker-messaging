
namespace Pipelines.Inbound;

public record struct PipelineFunctions<TCapabilities>
(
  AdvancePipeline AdvancePipeline,
  ExecuteOperation<TCapabilities> ExecuteOperationAsync,
  PropagateException PropagateException,
  CanFastRetry CanFastRetry
);