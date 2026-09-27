
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecutePublishingOperationAsync<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) => decision switch
  {
    PublishingActions.Mapping => ToTask(MapOutboxMessage(capabilities.Mapping, data)),
    PublishingActions.Producing => ToTask(ProduceEnvelope(capabilities.Producing, data)),
    PublishingActions.Publishing => PublishEnvelopeAsync(capabilities.Publishing, data, ct),
    PublishingActions.Scheduling => ScheduleOutboxMessageAsync(capabilities.Scheduling, data, ct),
    PublishingActions.Abandoning => AbandonOutboxMessageAsync(capabilities.Abandoning, data, ct),
    PublishingActions.Closing => CloseOutboxMessageAsync(capabilities.Closing, data, ct),
    _ => throw new InvalidOperationException($"Invalid execute operation decision {decision}")
  };
}
