namespace Pipelines.Inbound;

internal static class DeadLetteringEntries
{
  const string Scope = $"{nameof(DeadLetteringEntries)}";
  internal const string Start = $"{Scope}.{nameof(Start)}";
  internal const string End = $"{Scope}.{nameof(End)}";
}
