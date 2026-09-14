
namespace Operations.Inbound.DeadLetterEnvelope;

internal enum DispatchingStates
{
  DispatchingAck,
  DispatchingNotAck,
  DispatchingError
}
