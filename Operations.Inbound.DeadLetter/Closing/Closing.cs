
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async ValueTask<(TData, string, Exception?)> CloseDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var @params = new ClosingUpdate(DeadLetterMessageStatus.Published);

    await services.UpdateDeadLetterMessageAsync(message, @params, ct);

    return (data, ClosingSuccess, null);
  }

  static (TData, string, Exception?) CloseDeadLetterMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IClosingData<TKey, TPayload> =>
    (data, ClosingError, exception);

  internal static ValueTask<(TData, string, Exception?)> CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      CloseDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      CloseDeadLetterMessageError<TData, TKey, TPayload>,
      ct
    );
}
