namespace Pipelines.Inbound;

internal static class RedirectingEntries
{
  const string Scope = $"{nameof(RedirectingEntries)}";
  internal const string Start = $"{Scope}.{nameof(Start)}";
  internal const string End = $"{Scope}.{nameof(End)}";
}
