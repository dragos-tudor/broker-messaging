
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(TData, ClosingStates, Exception?)> CloseDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var @params = new ClosingUpdate(DeadLetterMessageStatus.Published);

    await services.UpdateDeadLetterMessageAsync(message, @params, ct);

    return (data, ClosingStates.Success, null);
  }

  static (TData, ClosingStates, Exception?) CloseDeadLetterMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IClosingData<TKey, TPayload> =>
    (data, ClosingStates.Error, exception);

  internal static Task<(TData, ClosingStates, Exception?)> CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
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
