
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(TData, AbandoningStates, Exception?)> AbandonDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var lastError = message.LastError;
    var status = DeadLetterMessageStatus.Abandoned;
    var @params = new AbandoningUpdate(status, lastError, null);

    await services.UpdateDeadLetterMessageAsync(message, @params, ct);

    return (data, AbandoningStates.Success, null);
  }

  static (TData, AbandoningStates, Exception?) AbandonDeadLetterMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IAbandoningData<TKey, TPayload> =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(TData, AbandoningStates, Exception?)> AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      AbandonDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      AbandonDeadLetterMessageError<TData, TKey, TPayload>,
      ct
    );

}
