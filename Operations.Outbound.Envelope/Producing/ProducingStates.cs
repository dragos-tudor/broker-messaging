
namespace Operations.Outbound.Envelope;

static partial class EnvelopeStates
{
  const string Scope = nameof(EnvelopeStates);
  internal const string Producing = $"{Scope}.{nameof(Producing)}";
  internal const string ProducingError = $"{Scope}.{nameof(ProducingError)}";
}
