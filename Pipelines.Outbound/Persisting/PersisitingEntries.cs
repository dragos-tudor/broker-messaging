namespace Pipelines.Outbound;

internal static class PersistingEntries
{
  const string Scope = $"{nameof(PersistingEntries)}";
  internal const string Start = $"{Scope}.{nameof(Start)}";
  internal const string End = $"{Scope}.{nameof(End)}";
}
