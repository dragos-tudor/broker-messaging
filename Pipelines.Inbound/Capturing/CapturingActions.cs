
namespace Pipelines.Inbound;

internal static class CapturingActions
{
  const string Scope = $"{nameof(CapturingActions)}";
  internal const string None = $"{Scope}.{nameof(None)}";
  internal const string Capturing = $"{Scope}.{nameof(Capturing)}";
  internal const string Verifying = $"{Scope}.{nameof(Verifying)}";
  internal const string Mapping = $"{Scope}.{nameof(Mapping)}";
  internal const string Validating = $"{Scope}.{nameof(Validating)}";
  internal const string Inserting = $"{Scope}.{nameof(Inserting)}";
  internal const string Confirming = $"{Scope}.{nameof(Confirming)}";
  internal const string ConfirmingFinal = $"{Scope}.{nameof(ConfirmingFinal)}";
}