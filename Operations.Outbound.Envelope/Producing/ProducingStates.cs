
namespace Operations.Outbound.Envelope;

static partial class EnvelopeStates
{
  const string Scope = "Envelope";
  internal const string ProducingEnqueue = $"{Scope}.{nameof(ProducingEnqueue)}";
  internal const string ProducingNotEnqueue = $"{Scope}.{nameof(ProducingNotEnqueue)}";
  internal const string ProducingError = $"{Scope}.{nameof(ProducingError)}";
}
