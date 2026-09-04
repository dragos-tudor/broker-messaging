
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string TransactingSuccess = $"{Scope}.{nameof(TransactingSuccess)}";
  internal const string TransactingError = $"{Scope}.{nameof(TransactingError)}";
  internal const string Idempotent = $"{Scope}.{nameof(Idempotent)}";
}
