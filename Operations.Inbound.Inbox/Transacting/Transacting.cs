
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    TransactInboxMessageSuccessAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      object?[] data,
      CancellationToken ct = default)
    where TSession : ISessionService
    {
      var message = RequireInboxMessage(GetInboxMessage<TKey, TPayload>(data));
      var model = RequireDomainModel(GetDomainModel<TKey, TPayload>(data));
      var @param = new TransactingUpdate(InboxMessageStatus.Handled);

      using var session = capabilities.GetSession();
      await capabilities.StoreDomainModelAsync(session, model, ct);
      await capabilities.UpdateInboxMessageAsync(session, message, @param, ct);
      await session.CompleteAsync(ct);

      return (data, TransactingStates.Success, null);
    }

  static (object?[], string, Exception?)
    TransactInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, TransactingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    TransactInboxMessageAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      object?[] data,
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
