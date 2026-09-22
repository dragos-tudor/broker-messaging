
namespace Pipelines.Inbound;

internal enum InboundPipelineTypes
{
  None = 0,
  Capturing,
  Redirecting,
  Handling,
  DeadLettering,
  Publishing,
  Dispatching
}