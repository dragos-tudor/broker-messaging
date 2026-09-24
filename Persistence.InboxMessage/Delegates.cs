
namespace Persistence.InboxMessage;

public delegate TSession GetSession<TSession>() where TSession: IDisposable;

public delegate InboxRetryOptions GetInboxRetryOptions();

public delegate Task<(object?, string?)> HandleInboxMessageAsync<TKey, TPayload>(
  IInboxMessage<TKey, TPayload> message,
  CancellationToken ct = default
);

public delegate Task<bool> InsertInboxMessageAsync<TKey, TPayload>(
  IInboxMessage<TKey, TPayload> message,
  CancellationToken ct = default
);

public delegate Task StoreDomainModelSessionAsync<TSession>(
  TSession session,
  object model,
  CancellationToken ct = default)
where TSession: IDisposable;

public delegate Task UpdateInboxMessageSessionAsync<TKey, TPayload, TParam, TSession>(
  TSession session,
  IInboxMessage<TKey, TPayload> message,
  TParam parameters,
  CancellationToken ct = default)
where TParam : struct
where TSession: IDisposable;

public delegate Task UpdateInboxMessageAsync<TKey, TPayload, TParam>(
  IInboxMessage<TKey, TPayload> message,
  TParam parameters,
  CancellationToken ct = default)
where TParam : struct;
