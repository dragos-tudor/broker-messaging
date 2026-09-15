
namespace Pipelines.Inbound;

internal enum PipelineTypes
{
  Capturing,
  Redirecting,
  Handling,
  DeadLettering,
  Publishing,
  Dispatching
}