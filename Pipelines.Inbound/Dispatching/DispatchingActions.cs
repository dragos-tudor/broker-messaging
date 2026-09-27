namespace Pipelines.Inbound;

internal static class DispatchingActions
{
  const string Scope = $"{nameof(DispatchingActions)}";
  internal const string Dispatching = $"{Scope}.{nameof(Dispatching)}";
  internal const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
}
