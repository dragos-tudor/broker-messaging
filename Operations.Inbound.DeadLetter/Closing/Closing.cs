
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    try {
      var message = RequireDeadLetterMessage(data.DeadLetterMessage);
      await services.UpdateDeadLetterMessageAsync(message, message =>
          SetDeadLetterMessageStatus(message, DeadLetterMessageStatus.Published),
          ct);

      return (data, CloseDeadLetterMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, CloseDeadLetterMessageErrorState, exception);
    }
  }
}
