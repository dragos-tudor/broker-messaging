using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public static partial class OutboundFuncs
{
  internal static string? PropagatePublishingException<TKey, TPayload>(
    object?[] data,
    string signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;
    var message = GetOutboxMessage<TKey, TPayload>(data);

    return signal switch
    {
      MappingStates.Error
      or PublishingStates.Error
      or ProducingStates.Error => message?.LastError = exception.Message,
      _ => default
    };
  }
}
