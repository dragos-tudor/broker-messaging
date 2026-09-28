
namespace Operations.Inbound.Inbox;

public sealed record TransactingCapabilities< TSession>(
  GetSession<TSession> GetSession,
  StoreDomainModelSessionAsync<TSession> StoreDomainModelAsync,
  UpdateInboxMessageSessionAsync< TransactingUpdate, TSession> UpdateInboxMessageAsync
)
where TSession : ISessionService;
