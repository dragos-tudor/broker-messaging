
namespace Persistence.InboxMessage;

public delegate TSession GetSession<TSession>() where TSession: IDisposable;

public delegate InboxRetryOptions GetInboxRetryOptions();

public delegate Task<(object?, string?)> HandleInboxMessageAsync(
  IInboxMessage message,
  CancellationToken ct = default
);

public delegate Task<bool> InsertInboxMessageAsync(
  IInboxMessage message,
  CancellationToken ct = default
);

public delegate Task StoreDomainModelSessionAsync<TSession>(
  TSession session,
  object model,
  CancellationToken ct = default)
where TSession: IDisposable;

public delegate Task UpdateInboxMessageSessionAsync<TParam, TSession>(
  TSession session,
  IInboxMessage message,
  TParam parameters,
  CancellationToken ct = default)
where TParam : struct
where TSession: IDisposable;

public delegate Task UpdateInboxMessageAsync<TParam>(
  IInboxMessage message,
  TParam parameters,
  CancellationToken ct = default)
where TParam : struct;

public delegate string? ValidateInboxMessage(IInboxMessage message);
