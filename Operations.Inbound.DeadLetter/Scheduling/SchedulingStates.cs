
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterStates
{
  internal const string SchedulingSuccess = $"{Scope}.{nameof(SchedulingSuccess)}";
  internal const string SchedulingExhausted = $"{Scope}.{nameof(SchedulingExhausted)}";
  internal const string SchedulingNotExhausted = $"{Scope}.{nameof(SchedulingNotExhausted)}";
  internal const string SchedulingError = $"{Scope}.{nameof(SchedulingError)}";
}
