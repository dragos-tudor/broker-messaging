
namespace Pipelines.Inbound;

internal enum InboundPipelineTypes
{
  Capturing,
  Redirecting,
  Handling,
  DeadLettering,
  Publishing,
  Dispatching
}