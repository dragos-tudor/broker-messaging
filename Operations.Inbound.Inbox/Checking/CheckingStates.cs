
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string CheckingRetryExhausted = $"{Scope}.{nameof(CheckingRetryExhausted)}";
  internal const string CheckingRetryNotExhausted = $"{Scope}.{nameof(CheckingRetryNotExhausted)}";
  internal const string CheckingRetryError = $"{Scope}.{nameof(CheckingRetryError)}";
}
