
namespace Operations.Inbound.DeadLetter;

internal static class SchedulingStates
{
  const string Scope = $"{nameof(SchedulingStates)}";
  internal const string Exhausted = $"{Scope}.{nameof(Exhausted)}";
  internal const string NotExhausted = $"{Scope}.{nameof(NotExhausted)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
