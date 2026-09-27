namespace Pipelines.Outbound;

internal static class DispatchingEntries
{
  const string Scope = $"{nameof(DispatchingEntries)}";
  internal const string Start = $"{Scope}.{nameof(Start)}";
  internal const string End = $"{Scope}.{nameof(End)}";
}
