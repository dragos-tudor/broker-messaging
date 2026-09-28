using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public static partial class InboundFuncs
{
  internal static string? PropagatePublishingException(
    object?[] data,
    string signal,
    Exception? exception)
  {
    if (exception is null) return default;
    if (exception is OperationCanceledException) return default;
    var message = GetDeadLetterMessage(data);

    return signal switch
    {
      MappingStates.Error
        or PublishingStates.Error
        or ProducingStates.Error => message?.LastError = exception.Message,
      _ => default
    };
  }
}
