
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static (TData, DispatchingStates, Exception?) DispatchDeadLetterEnvelopeSuccess<TServices, TData>(
    TServices services,
    TData data)
  where TServices : IDispatchingServices
  where TData : IDispatchingData
  {
    var result = RequireProduceResult(data.ProduceResult);

    return result.IsAcknowledged?
      (data, DispatchingStates.Ack, null):
      (data, DispatchingStates.NotAck, null);
  }

  static (TData, DispatchingStates, Exception?) DispatchDeadLetterEnvelopeError<TData>(
    TData data,
    Exception exception)
  where TData : IDispatchingData =>
    (data, DispatchingStates.Error, exception);

  internal static ValueTask<(TData, DispatchingStates, Exception?)> DispatchDeadLetterEnvelope<TServices, TData>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IDispatchingServices
  where TData : IDispatchingData =>
    TryCatch(
      services,
      data,
      DispatchDeadLetterEnvelopeSuccess,
      DispatchDeadLetterEnvelopeError
    );
}
