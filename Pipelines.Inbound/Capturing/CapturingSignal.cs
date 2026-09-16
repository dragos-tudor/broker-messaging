using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

internal enum CapturingEntry { Start }

internal readonly union CapturingSignal(
  CapturingEntry,
  CapturingStates,
  VerifyingStates,
  MappingStates,
  ValidatingStates,
  InsertingStates,
  ConfirmingStates,
  ConfirmingFinalStates
);
