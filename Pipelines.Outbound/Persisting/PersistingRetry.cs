
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static bool CanFastRetryPersisting(
    PersistingSignal signal) =>
    false;
}