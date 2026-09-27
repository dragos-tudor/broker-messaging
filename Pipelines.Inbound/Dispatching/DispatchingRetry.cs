using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static bool CanFastRetryDispatching(string signal) => signal switch
  {
    SchedulingStates.Error => true,
    AbandoningStates.Error => true,
    ClosingStates.Error => true,
    _ => false
  };
}
