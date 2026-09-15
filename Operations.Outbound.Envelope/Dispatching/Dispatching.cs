
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  internal static (TData, DispatchingStates, Exception?) DispatchEnvelopeSuccess<TServices, TData>(
    TServices services,
    TData data)
  where TServices : IDispatchingServices
  where TData : IDispatchingData
  {
    var result = RequireProduceResult(data.ProduceResult);

    return result.IsAcknowledged ?
      (data, DispatchingStates.Ack, null) :
      (data, DispatchingStates.NotAck, null);
  }

  static (TData, DispatchingStates, Exception?) DispatchEnvelopeError<TData>(
    TData data,
    Exception exception)
  where TData : IDispatchingData =>
    (data, DispatchingStates.Error, exception);

  internal static ValueTask<(TData, DispatchingStates, Exception?)> DispatchEnvelope<TServices, TData>(
    TServices services,
    TData data,
    CancellationToken _ = default)
  where TServices : IDispatchingServices
  where TData : IDispatchingData =>
    TryCatch(
      services,
      data,
      DispatchEnvelopeSuccess,
      DispatchEnvelopeError
    );
}
