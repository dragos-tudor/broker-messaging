namespace Pipelines.Outbound;

internal static class PublishingEntries
{
  const string Scope = $"{nameof(PublishingEntries)}";
  internal const string Start = $"{Scope}.{nameof(Start)}";
  internal const string End = $"{Scope}.{nameof(End)}";
}
