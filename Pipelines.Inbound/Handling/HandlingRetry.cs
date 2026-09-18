
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static bool CanFastRetryHandling(
    HandlingSignal signal) =>
    signal switch
    {
      HandlingStates.Error => true,
      SchedulingStates.Error => true,
      AbandoningStates.Error => true,

      _ => false
    };
}