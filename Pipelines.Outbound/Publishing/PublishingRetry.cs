
using Operations.Outbound.Envelope;
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static bool CanFastRetryPublishing(
    PublishingSignal signal) =>
    signal switch
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