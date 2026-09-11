
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string DeadLetteringSuccess = $"{Scope}.{nameof(DeadLetteringSuccess)}";
  internal const string DeadLetteringError = $"{Scope}.{nameof(DeadLetteringError)}";
}
