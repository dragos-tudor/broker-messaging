
namespace Operations.Inbound.DeadLetterEnvelope;

internal enum ProducingStates
{
  ProducingEnqueue,
  ProducingNotEnqueue,
  ProducingError
}
