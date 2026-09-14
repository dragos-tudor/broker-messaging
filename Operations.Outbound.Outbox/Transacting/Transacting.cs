
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async ValueTask<(TData, TransactingStates, Exception?)> TransactOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload, TSession>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ITransactingServices<TKey, TPayload, TSession>
  where TData : ITransactingData<TKey, TPayload>
  where TSession : IDisposable
  {
    var message = RequireOutboxMessage(data.OutboxMessage);
    var model = RequireDomainModel(data.DomainModel);
    var @params = (model, message);

    using var session = services.GetSession();
    await services.TransactSessionAsync(
      services,
      session,
      @params,
        static (services, session, @params, ct) =>
          services.PersistDomainModelAsync(session, @params.model, ct),
        static (services, session, @params, ct) =>
          services.InsertOutboxMessageAsync(session, @params.message, ct),
      ct
    );

    return (data, TransactingSuccess, null);
  }

  static (TData, TransactingStates, Exception?) TransactOutboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : ITransactingData<TKey, TPayload>  =>
    (data, TransactingError, exception);

  internal static ValueTask<(TData, TransactingStates, Exception?)> TransactOutboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ITransactingServices<TKey, TPayload, TSession>
  where TData : ITransactingData<TKey, TPayload>
  where TSession : IDisposable =>
    TryCatch(
      services,
      data,
      TransactOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload, TSession>,
      TransactOutboxMessageError<TData, TKey, TPayload>,
      ct);

}
