
namespace Pipelines.Inbound;

public enum InboundPipelineTypes
{
  Capturing,
  Redirecting,
  Handling,
  DeadLettering,
  Publishing,
  Dispatching
}