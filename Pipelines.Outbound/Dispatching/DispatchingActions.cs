
namespace Pipelines.Outbound;

static class DispatchingActions
{
  internal const string Scope = "Dispatching";
  internal const string Dispatching = $"{Scope}.{nameof(Dispatching)}";
  internal const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
}
