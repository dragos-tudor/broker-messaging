
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static bool CanFastRetryCapturing(string signal) =>
    signal switch
    {
      CapturingStates.Error => true,
      InsertingStates.Error => true,
      ConfirmingStates.Error => true,
      ConfirmingFinalStates.Error => true,

      _ => false
    };
}