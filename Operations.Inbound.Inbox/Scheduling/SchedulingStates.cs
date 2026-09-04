
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string SchedulingExhausted = $"{Scope}.{nameof(SchedulingExhausted)}";
  internal const string SchedulingNotExhausted = $"{Scope}.{nameof(SchedulingNotExhausted)}";
  internal const string SchedulingError = $"{Scope}.{nameof(SchedulingError)}";
}
