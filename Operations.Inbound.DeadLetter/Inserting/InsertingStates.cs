
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterStates
{
  internal const string InsertingSuccess = $"{Scope}.{nameof(InsertingSuccess)}";
  internal const string InsertingError = $"{Scope}.{nameof(InsertingError)}";
  internal const string Idempotent = $"{Scope}.{nameof(Idempotent)}";
}
