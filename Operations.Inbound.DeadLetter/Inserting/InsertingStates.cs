
namespace Operations.Inbound.DeadLetter;

internal enum InsertingStates
{
  Success,
  Error,
  Idempotent
}
