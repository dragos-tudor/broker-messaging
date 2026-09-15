
namespace Operations.Inbound.DeadLetterEnvelope;

internal enum ProducingStates
{
  Enqueue,
  NotEnqueue,
  Error
}
