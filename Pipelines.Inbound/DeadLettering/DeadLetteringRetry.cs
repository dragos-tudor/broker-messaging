
using Operations.Inbound.DeadLetter;
using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static bool CanFastRetryDeadLettering(
    DeadLetteringSignal signal) =>
    signal switch
    {
      InsertingStates.Error => true,
      Inbox.ClosingStates.Error => true,
      Inbox.AbandoningStates.Error => true,

      _ => false
    };
}