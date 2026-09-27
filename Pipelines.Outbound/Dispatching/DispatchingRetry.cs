using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static bool CanFastRetryDispatching(string signal) => signal switch
  {
    SchedulingStates.Error => true,
    ClosingStates.Error => true,
    AbandoningStates.Error => true,
    _ => false
  };
}
