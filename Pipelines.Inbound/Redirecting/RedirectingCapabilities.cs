using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

public sealed record RedirectingCapabilities
(
  Operations.Inbound.DeadLetterEnvelope.RedirectingCapabilities Redirecting,
  ConvertingCapabilities Converting,
  ConfirmingCapabilities Confirming
);