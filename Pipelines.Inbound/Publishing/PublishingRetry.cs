
using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
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