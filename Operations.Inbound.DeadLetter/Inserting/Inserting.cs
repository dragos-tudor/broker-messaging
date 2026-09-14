
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async ValueTask<(TData, InsertingStates, Exception?)> InsertDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);

    return await services.InsertDeadLetterMessageAsync(message, ct)?
      (data, InsertingSuccess, null):
      (data, InsertingIdempotent, null);
  }

  static (TData, InsertingStates, Exception?) InsertDeadLetterMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IInsertingData<TKey, TPayload> =>
    (data, InsertingError, exception);

  internal static ValueTask<(TData, InsertingStates, Exception?)> InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      InsertDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      InsertDeadLetterMessageError<TData, TKey, TPayload>,
      ct
    );
}
