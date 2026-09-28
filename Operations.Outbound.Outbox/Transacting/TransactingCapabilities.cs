namespace Operations.Outbound.Outbox;

public sealed record TransactingCapabilities<TSession>(
  GetOutboxSession<TSession> GetSession,
  StoreDomainModelSessionAsync<TSession> PersistDomainModelAsync,
  InsertOutboxMessageSessionAsync<TSession> InsertOutboxMessageAsync
)
where TSession : ISessionService;
