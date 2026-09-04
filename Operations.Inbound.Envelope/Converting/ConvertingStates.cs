
namespace Operations.Inbound.Envelope;

static partial class EnvelopeStates
{
  internal const string ConvertingSuccess = $"{Scope}.{nameof(ConvertingSuccess)}";
  internal const string ConvertingInvalid = $"{Scope}.{nameof(ConvertingInvalid)}";
  internal const string ConvertingError = $"{Scope}.{nameof(ConvertingError)}";
}
