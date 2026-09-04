
namespace Operations.Inbound.DeadLetter;

internal static partial class DeadLetterStates
{
  const string Scope = nameof(DeadLetterStates);
  internal const string AbandoningSuccess = $"{Scope}.{nameof(AbandoningSuccess)}";
  internal const string AbandoningError = $"{Scope}.{nameof(AbandoningError)}";
}