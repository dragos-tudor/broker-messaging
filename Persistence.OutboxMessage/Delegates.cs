namespace Persistence.OutboxMessage;

public delegate OutboxRetryOptions GetOutboxRetryOptions();

public delegate TSession GetOutboxSession<TSession>() where TSession : IDisposable;

public delegate Task<bool> InsertOutboxMessageSessionAsync<TSession>(
  TSession session,
  IOutboxMessage message,
  CancellationToken ct = default
)
where TSession : IDisposable;

public delegate Task StoreDomainModelSessionAsync<TSession>(
  TSession session,
  object model,
  CancellationToken ct = default
)
where TSession : IDisposable;

public delegate Task UpdateOutboxMessageAsync<TParam>(
  IOutboxMessage message,
  TParam parameters,
  CancellationToken ct = default
)
where TParam : struct;

public delegate string? ValidateOutboxMessage(IOutboxMessage message);
