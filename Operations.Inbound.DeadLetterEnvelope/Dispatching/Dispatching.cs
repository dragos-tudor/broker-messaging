
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static (TData, string, Exception?) DispatchDeadLetterEnvelopeSuccess<TServices, TData>(
    TServices services,
    TData data)
  where TServices : IDispatchingServices
  where TData : IDispatchingData
  {
    var result = RequireProduceResult(data.ProduceResult);

    return result.IsAcknowledged?
      (data, DispatchingAck, null):
      (data, DispatchingNotAck, null);
  }

  static (TData, string, Exception?) DispatchDeadLetterEnvelopeError<TData>(
    TData data,
    Exception exception)
  where TData : IDispatchingData =>
    (data, DispatchingError, exception);

  internal static ValueTask<(TData, string, Exception?)> DispatchDeadLetterEnvelope<TServices, TData>(
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
