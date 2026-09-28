namespace Pipelines.Inbound;

internal static class HandlingActions
{
  internal const string Scope = $"{nameof(HandlingActions)}";
  internal const string Handling = $"{Scope}.{nameof(Handling)}";
  internal const string Transacting = $"{Scope}.{nameof(Transacting)}";
  internal const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
}
