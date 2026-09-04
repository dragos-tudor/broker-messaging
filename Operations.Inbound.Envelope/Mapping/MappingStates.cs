
namespace Operations.Inbound.Envelope;

static partial class EnvelopeStates
{
  internal const string MappingSuccess = $"{Scope}.{nameof(MappingSuccess)}";
  internal const string MappingError = $"{Scope}.{nameof(MappingError)}";
  internal const string MappingValueError = $"{Scope}.{nameof(MappingValueError)}";
}
