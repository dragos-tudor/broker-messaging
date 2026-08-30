
namespace Persistence.DeadLetterMessage;

public interface IDeadLetterMessageInsertService<TKey, TPayload>
{
  internal Task<bool> InsertDeadLetterMessageAsync(
    DeadLetterMessage<TKey, TPayload> message,
    CancellationToken ct = default);
}

public interface IDeadLetterMessageOptionsReaderService {
  DeadLetterMessageOptions GetDeadLetterMessageOptions();
}

public interface IDeadLetterMessagePayloadMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload> {
  TValue FromDeadLetterMessagePayload(TPayload value);
}

public interface IDeadLetterMessageQueueReaderService<TKey, TPayload> {
  string GetDeadLetterQueueName(DeadLetterMessage<TKey, TPayload> message);
}

public interface IDeadLetterMessageUpdateService<TKey, TPayload>
{
  Task UpdateDeadLetterMessageAsync<TMessage>(
    TMessage message,
    Func<TMessage, TMessage> update,
    CancellationToken ct = default) where TMessage : DeadLetterMessage<TKey, TPayload>;
}
