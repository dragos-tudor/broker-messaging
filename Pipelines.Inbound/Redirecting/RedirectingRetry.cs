
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static bool CanFastRetryRedirecting(
    RedirectingSignal signal) =>
    signal switch
    {
      RedirectingStates.Error => true,
      _ => false
    };
}