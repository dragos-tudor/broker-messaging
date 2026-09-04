
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string MappingSuccess = $"{Scope}.{nameof(MappingSuccess)}";
  internal const string MappingError = $"{Scope}.{nameof(MappingError)}";
  internal const string MappingPayloadError = $"{Scope}.{nameof(MappingPayloadError)}";
}
