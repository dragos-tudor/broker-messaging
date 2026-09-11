
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  internal const string DispatchingAck = $"{Scope}.{nameof(DispatchingAck)}";
  internal const string DispatchingNotAck = $"{Scope}.{nameof(DispatchingNotAck)}";
  internal const string DispatchingError = $"{Scope}.{nameof(DispatchingError)}";
}
