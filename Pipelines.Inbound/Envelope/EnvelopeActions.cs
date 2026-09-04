
namespace Pipelines.Inbound;

static class EnvelopeActions
{
  internal const string Scope = "Envelope";
  internal const string Capturing = $"{Scope}.{nameof(Capturing)}";
  internal const string Validating = $"{Scope}.{nameof(Validating)}";
  internal const string Mapping = $"{Scope}.{nameof(Mapping)}";
  internal const string Mapped = $"{Scope}.{nameof(Mapped)}";
  internal const string Converting = $"{Scope}.{nameof(Converting)}";
  internal const string Converted = $"{Scope}.{nameof(Converted)}";
  internal const string Confirming = $"{Scope}.{nameof(Confirming)}";
  internal const string Confirmed = $"{Scope}.{nameof(Confirmed)}";
}