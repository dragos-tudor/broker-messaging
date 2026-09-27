
namespace Pipelines.Inbound;

internal static class PipelineTypes
{
  const string Scope = $"{nameof(PipelineTypes)}";
  internal const string Capturing = $"{Scope}{nameof(Capturing)}";
  internal const string Redirecting = $"{Scope}{nameof(Redirecting)}";
  internal const string Handling = $"{Scope}{nameof(Handling)}";
  internal const string DeadLettering = $"{Scope}{nameof(DeadLettering)}";
  internal const string Publishing = $"{Scope}{nameof(Publishing)}";
  internal const string Dispatching = $"{Scope}{nameof(Dispatching)}";
}