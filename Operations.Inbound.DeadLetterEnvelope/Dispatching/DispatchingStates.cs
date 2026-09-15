
namespace Operations.Inbound.DeadLetterEnvelope;

internal enum DispatchingStates
{
  Ack,
  NotAck,
  Error
}
