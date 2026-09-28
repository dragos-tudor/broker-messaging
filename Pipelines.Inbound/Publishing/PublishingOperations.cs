
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecutePublishingOperationAsync(
      PublishingCapabilities capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) => decision switch
  {
    PublishingActions.Mapping => ToTask(MapDeadLetterMessage(capabilities.Mapping, data)),
    PublishingActions.Publishing => PublishDeadLetterEnvelopeAsync(capabilities.Publishing, data, ct),
    PublishingActions.Producing => ToTask(ProduceDeadLetterEnvelope(capabilities.Producing, data)),
    PublishingActions.Scheduling => ScheduleDeadLetterMessageAsync(capabilities.Scheduling, data, ct),
    PublishingActions.Abandoning => AbandonDeadLetterMessageAsync(capabilities.Abandoning, data, ct),
    PublishingActions.Closing => CloseDeadLetterMessageAsync(capabilities.Closing, data, ct),
    _ => ToTask((data, PublishingEntries.End, default(Exception?)))
  };
}
