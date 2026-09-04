
namespace Operations.Inbound.Envelope;

static partial class EnvelopeStates
{
  const string Scope = nameof(EnvelopeStates);
  internal const string CapturingSuccess = $"{Scope}.{nameof(CapturingSuccess)}";
  internal const string CapturingNotCaptured = $"{Scope}.{nameof(CapturingNotCaptured)}";
  internal const string CapturingError = $"{Scope}.{nameof(CapturingError)}";
}
