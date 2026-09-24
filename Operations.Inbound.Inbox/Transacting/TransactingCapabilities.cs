
namespace Operations.Inbound.Inbox;

sealed record TransactingCapabilities<TKey, TPayload, TSession>(
  GetSession<TSession> GetSession,
  StoreDomainModelSessionAsync<TSession> StoreDomainModelAsync,
  UpdateInboxMessageSessionAsync<TKey, TPayload, TransactingUpdate, TSession> UpdateInboxMessageAsync
)
where TSession : ISessionService;
