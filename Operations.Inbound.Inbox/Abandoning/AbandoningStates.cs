
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  const string Scope = nameof(InboxStates);
  internal const string AbandoningSuccess = $"{Scope}.{nameof(AbandoningSuccess)}";
  internal const string AbandoningError = $"{Scope}.{nameof(AbandoningError)}";
}
