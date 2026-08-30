using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload>
  {
    try
    {
      var message = RequireDeadLetterMessage(data.DeadLetterMessage);
      var error = data.PipelineError ?? "Unknown abandoning dead letter message error";

      await services.UpdateDeadLetterMessageAsync(message, message =>
          SetDeadLetterMessageStatus(message, DeadLetterMessageStatus.Abandoned)
              .SetDeadLetterMessageLastError(error),
          ct);

      return (data, AbandonDeadLetterMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      return (data, AbandonDeadLetterMessageErrorState, exception);
    }
  }
}
