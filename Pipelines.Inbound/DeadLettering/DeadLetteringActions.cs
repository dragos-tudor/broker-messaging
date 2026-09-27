namespace Pipelines.Inbound;

internal static class DeadLetteringActions
{
  const string Scope = $"{nameof(DeadLetteringActions)}";
  internal const string Converting = $"{Scope}.{nameof(Converting)}";
  internal const string Inserting = $"{Scope}.{nameof(Inserting)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
}
