
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  const string Scope = nameof(DeadLetterEnvelopeStates);
  internal const string CheckingRetryExhausted = $"{Scope}.{nameof(CheckingRetryExhausted)}";
  internal const string CheckingRetryNotExhausted = $"{Scope}.{nameof(CheckingRetryNotExhausted)}";
  internal const string CheckingRetryError = $"{Scope}.{nameof(CheckingRetryError)}";

}
