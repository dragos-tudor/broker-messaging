
namespace Pipelines.Inbound;

static class PipelineTypes
{
  internal const string Capturing = $"{nameof(Capturing)}";
  internal const string Redirecting = $"{nameof(Redirecting)}";
  internal const string Handling = $"{nameof(Handling)}";
  internal const string DeadLettering = $"{nameof(DeadLettering)}";
  internal const string Publishing = $"{nameof(Publishing)}";
  internal const string Dispatching = $"{nameof(Dispatching)}";
}