
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(TransactingData<TKey, TPayload>, TransactingStates, Exception?)>
    TransactInboxMessageSuccessAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      TransactingData<TKey, TPayload> data,
      CancellationToken ct = default)
    where TSession : ISessionService
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var model = RequireDomainModel(data.Model);
      var @param = new TransactingUpdate(InboxMessageStatus.Handled);

      using var session = capabilities.GetSession();
      await capabilities.StoreDomainModelAsync(session, model, ct);
      await capabilities.UpdateInboxMessageAsync(session, message, @param, ct);
      await session.CompleteAsync(ct);

      return (data, TransactingStates.Success, null);
    }

  static (TransactingData<TKey, TPayload>, TransactingStates, Exception?)
    TransactInboxMessageError<TKey, TPayload>(
      TransactingData<TKey, TPayload> data,
      Exception exception) =>
    (data, TransactingStates.Error, exception);

  internal static Task<(TransactingData<TKey, TPayload>, TransactingStates, Exception?)>
    TransactInboxMessageAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      TransactingData<TKey, TPayload> data,
      CancellationToken ct = default)
    where TSession : ISessionService =>
    TryCatch(
      capabilities,
      data,
      TransactInboxMessageSuccessAsync,
      TransactInboxMessageError,
      ct
    );
}
