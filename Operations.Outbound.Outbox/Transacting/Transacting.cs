using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> TransactOutboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ITransactingServices<TKey, TPayload, TSession>
  where TData : ITransactingData<TKey, TPayload>
  where TSession : IDisposable
  {
    try {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var model = RequireOutboxModel(data.Model);

      using var session = services.GetSession();
      await services.TransactSessionAsync(
        session,
        (session) => services.PersistOutboxModelAsync(session, model),
        (session) => services.InsertOutboxMessageAsync(session,
          SetOutboxMessageStatus(message, OutboxMessageStatus.Processing),
          ct),
        ct
      );

      return (data, TransactOutboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, TransactOutboxMessageErrorState, exception);
    }
  }
}