namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(TransactingData<TKey, TPayload>, TransactingStates, Exception?)>
    TransactOutboxMessageSuccessAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      TransactingData<TKey, TPayload> data,
      CancellationToken ct = default)
    where TSession : ISessionService
    {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var model = RequireDomainModel(data.Model);

      using var session = capabilities.GetSession();
      await capabilities.PersistDomainModelAsync(session, model, ct);
      await capabilities.InsertOutboxMessageAsync(session, message, ct);
      await session.CompleteAsync(ct);

      return (data, TransactingStates.Success, null);
    }

  static (TransactingData<TKey, TPayload>, TransactingStates, Exception?)
    TransactOutboxMessageError<TKey, TPayload>(
      TransactingData<TKey, TPayload> data,
      Exception exception) =>
    (data, TransactingStates.Error, exception);

  internal static Task<(TransactingData<TKey, TPayload>, TransactingStates, Exception?)>
    TransactOutboxMessageAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      TransactingData<TKey, TPayload> data,
      CancellationToken ct = default)
    where TSession : ISessionService =>
    TryCatch(
      capabilities,
      data,
      TransactOutboxMessageSuccessAsync,
      TransactOutboxMessageError,
      ct);
}
