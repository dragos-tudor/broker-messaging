
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string HandlingSuccess = $"{Scope}.{nameof(HandlingSuccess)}";
  internal const string HandlingDomainError = $"{Scope}.{nameof(HandlingDomainError)}";
  internal const string HandlingExhausted = $"{Scope}.{nameof(HandlingExhausted)}";
  internal const string HandlingError = $"{Scope}.{nameof(HandlingError)}";
}
