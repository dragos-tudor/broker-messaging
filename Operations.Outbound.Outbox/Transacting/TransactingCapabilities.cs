namespace Operations.Outbound.Outbox;

public sealed record TransactingCapabilities<TKey, TPayload, TSession>(
  GetOutboxSession<TSession> GetSession,
  StoreDomainModelSessionAsync<TSession> PersistDomainModelAsync,
  InsertOutboxMessageSessionAsync<TKey, TPayload, TSession> InsertOutboxMessageAsync
)
where TSession : ISessionService;
