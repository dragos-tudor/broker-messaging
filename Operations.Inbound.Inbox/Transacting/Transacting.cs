
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ITransactingServices<TKey, TPayload, TSession>
  where TData : ITransactingData<TKey, TPayload>
  where TSession : IDisposable
  {
    try
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var model = RequireInboxModel(data.Model);

      using var session = services.GetSession();
      await services.TransactSessionAsync(
        session,
        (session) => services.PersistInboxModelAsync(session, model),
        (session) => services.UpdateInboxMessageAsync(session, message,
          message => SetInboxMessageStatus(message, InboxMessageStatus.Handled)),
        ct
      );

      return (data, TransactingSuccess, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, TransactingError, exception);
    }
  }
}
