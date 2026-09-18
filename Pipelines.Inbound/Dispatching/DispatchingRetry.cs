
using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static bool CanFastRetryDispatching(
    DispatchingSignal signal) =>
    signal switch
    {
      SchedulingStates.Error => true,
      ClosingStates.Error => true,
      AbandoningStates.Error => true,

      _ => false
    };
}