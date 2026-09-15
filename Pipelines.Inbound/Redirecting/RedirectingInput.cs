using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal enum RedirectingEntry { Start }

internal readonly union RedirectingInput(
  RedirectingEntry,
  Envelope.ConvertingStates,
  DeadLetterEnvelope.RedirectingStates,
  Envelope.ConfirmingFinalStates
);
