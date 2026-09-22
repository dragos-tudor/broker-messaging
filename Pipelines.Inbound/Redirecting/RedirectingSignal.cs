using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal readonly union RedirectingSignal(
  RedirectingEntries,
  Envelope.ConvertingStates,
  DeadLetterEnvelope.RedirectingStates,
  Envelope.ConfirmingFinalStates
);
