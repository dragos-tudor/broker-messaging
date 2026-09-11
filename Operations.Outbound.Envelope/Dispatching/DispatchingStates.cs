
namespace Operations.Outbound.Envelope;

static partial class EnvelopeStates
{
  internal const string DispatchingAck = $"{Scope}.{nameof(DispatchingAck)}";
  internal const string DispatchingNotAck = $"{Scope}.{nameof(DispatchingNotAck)}";
  internal const string DispatchingError = $"{Scope}.{nameof(DispatchingError)}";
}
