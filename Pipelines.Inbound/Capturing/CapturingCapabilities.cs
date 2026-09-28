
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public sealed record CapturingCapabilities(
  Operations.Inbound.Envelope.CapturingCapabilities Capturing,
  VerifyingCapabilities Verifying,
  MappingCapabilities Mapping,
  ValidatingCapabilities Validating,
  InsertingCapabilities Inserting,
  ConfirmingCapabilities Confirming
);