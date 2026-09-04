
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string InsertingSuccess = $"{Scope}.{nameof(InsertingSuccess)}";
  internal const string InsertingError = $"{Scope}.{nameof(InsertingError)}";
  internal const string Idempotent = $"{Scope}.{nameof(Idempotent)}";
  internal const string InsertingExhausted = $"{Scope}.{nameof(InsertingExhausted)}";
}
