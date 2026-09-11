
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string InsertingSuccess = $"{Scope}.{nameof(InsertingSuccess)}";
  internal const string InsertingError = $"{Scope}.{nameof(InsertingError)}";
  internal const string InsertingIdempotent = $"{Scope}.{nameof(InsertingIdempotent)}";
}
