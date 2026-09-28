namespace Pipelines.Outbound;

internal static class PersistingActions
{
  internal const string Scope = $"{nameof(PersistingActions)}";
  internal const string Validating = $"{Scope}.{nameof(Validating)}";
  internal const string Transacting = $"{Scope}.{nameof(Transacting)}";
}
