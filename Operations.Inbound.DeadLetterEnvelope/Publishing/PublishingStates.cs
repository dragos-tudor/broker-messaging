
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  internal const string PublishingSuccess = $"{Scope}.{nameof(PublishingSuccess)}";
  internal const string PublishingError = $"{Scope}.{nameof(PublishingError)}";
}
