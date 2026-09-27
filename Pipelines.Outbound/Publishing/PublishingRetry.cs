using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static bool CanFastRetryPublishing(string signal) => signal switch
  {
    PublishingStates.Error => true,
    ProducingStates.NotEnqueue => true,
    ProducingStates.Error => true,
    SchedulingStates.Error => true,
    ClosingStates.Error => true,
    AbandoningStates.Error => true,
    _ => false
  };
}
