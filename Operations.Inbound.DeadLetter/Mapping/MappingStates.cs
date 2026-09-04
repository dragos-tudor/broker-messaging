
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterStates
{
  internal const string MappingSuccess = $"{Scope}.{nameof(MappingSuccess)}";
  internal const string MappingError = $"{Scope}.{nameof(MappingError)}";
  internal const string MappingPayloadError = $"{Scope}.{nameof(MappingPayloadError)}";
}
