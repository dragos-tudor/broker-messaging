
namespace Operations.Inbound.Envelope;

static partial class EnvelopeStates
{
  internal const string ConfirmingSuccess = $"{Scope}.{nameof(ConfirmingSuccess)}";
  internal const string ConfirmingFinalSuccess = $"{Scope}.{nameof(ConfirmingFinalSuccess)}";
  internal const string ConfirmingError = $"{Scope}.{nameof(ConfirmingError)}";
  internal const string ConfirmingFinalError = $"{Scope}.{nameof(ConfirmingFinalError)}";
}
