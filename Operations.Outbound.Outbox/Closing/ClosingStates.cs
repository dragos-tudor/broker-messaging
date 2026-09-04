
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string ClosingSuccess = $"{Scope}.{nameof(ClosingSuccess)}";
  internal const string ClosingError = $"{Scope}.{nameof(ClosingError)}";
}
