
namespace Pipelines.Outbound;

static class PersistingActions
{
  internal const string Scope = "Persisting";
  internal const string Validating = $"{Scope}.{nameof(Validating)}";
  internal const string Transacting = $"{Scope}.{nameof(Transacting)}";
}
