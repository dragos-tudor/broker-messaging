
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(TData, TransactingStates, Exception?)> TransactInboxMessageSuccessAsync<TServices, TData, TKey, TPayload, TSession>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ITransactingServices<TKey, TPayload, TSession>
  where TData : ITransactingData<TKey, TPayload>
  where TSession : IDisposable
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var model = RequireDomainModel(data.DomainModel);
    var @params = (model, message);

    using var session = services.GetSession();
    await services.TransactSessionAsync(
      services,
      session,
      @params,
      static (services, session, @params, ct) =>
        services.StoreDomainModelAsync(session, @params.model, ct),
      static (services, session, @params, ct) =>
        services.UpdateInboxMessageAsync(session, @params.message,
          new TransactingUpdate(InboxMessageStatus.Handled), ct),
      ct
    );

    return (data, TransactingStates.Success, null);
  }

  static (TData, TransactingStates, Exception?) TransactInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : ITransactingData<TKey, TPayload> =>
    (data, TransactingStates.Error, exception);

  internal static Task<(TData, TransactingStates, Exception?)> TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ITransactingServices<TKey, TPayload, TSession>
  where TData : ITransactingData<TKey, TPayload>
  where TSession : IDisposable =>
    TryCatch(
      services,
      data,
      TransactInboxMessageSuccessAsync<TServices, TData, TKey, TPayload, TSession>,
      TransactInboxMessageError<TData, TKey, TPayload>,
      ct
    );

}
