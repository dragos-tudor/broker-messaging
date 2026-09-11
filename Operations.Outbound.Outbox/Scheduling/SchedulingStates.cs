
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string SchedulingExhausted = $"{Scope}.{nameof(SchedulingExhausted)}";
  internal const string SchedulingNotExhausted = $"{Scope}.{nameof(SchedulingNotExhausted)}";
  internal const string SchedulingError = $"{Scope}.{nameof(SchedulingError)}";
}
