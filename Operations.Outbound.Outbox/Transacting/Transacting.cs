namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    TransactOutboxMessageSuccessAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      object?[] data,
      CancellationToken ct = default)
    where TSession : ISessionService
  {
    var message = RequireOutboxMessage(GetOutboxMessage<TKey, TPayload>(data));
    var model = RequireDomainModel(GetDomainModel(data));

    using var session = capabilities.GetSession();
    await capabilities.PersistDomainModelAsync(session, model, ct);
    await capabilities.InsertOutboxMessageAsync(session, message, ct);
    await session.CompleteAsync(ct);

    return (data, TransactingStates.Success, null);
  }

  static (object?[], string, Exception?)
    TransactOutboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, TransactingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    TransactOutboxMessageAsync<TKey, TPayload, TSession>(
      TransactingCapabilities<TKey, TPayload, TSession> capabilities,
      object?[] data,
      CancellationToken ct = default)
    where TSession : ISessionService =>
    TryCatch(
      capabilities,
      data,
      TransactOutboxMessageSuccessAsync,
      TransactOutboxMessageError,
      ct);
}
