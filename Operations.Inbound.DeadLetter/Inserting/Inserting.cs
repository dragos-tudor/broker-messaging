
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
  {
    try
    {
      var deadLetterMessage = RequireDeadLetterMessage(data.DeadLetterMessage);

      var deadLetterInserted = await services.InsertDeadLetterMessageAsync(deadLetterMessage, ct);
      return deadLetterInserted
        ? (data, InsertingSuccess, null)
        : (data, Idempotent, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      return (data, InsertingError, exception);
    }
  }
}
