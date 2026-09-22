using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

internal readonly union CapturingSignal(
  CapturingEntries,
  CapturingStates,
  VerifyingStates,
  MappingStates,
  ValidatingStates,
  InsertingStates,
  ConfirmingStates,
  ConfirmingFinalStates
);
