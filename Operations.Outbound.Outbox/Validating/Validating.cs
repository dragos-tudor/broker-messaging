using Funcs = Persistence.OutboxMessage.OutboxMessageFuncs;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (TData, ValidatingStates, Exception?) ValidateOutboxMessageSuccess<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data)
  where TServices : IValidatingServices<TKey, TPayload>
  where TData : IValidatingData<TKey, TPayload>
  {
    var message = RequireOutboxMessage(data.OutboxMessage);

    var error = Funcs.ValidateOutboxMessage(message);
    return error is null ?
        (data, ValidatingStates.Success, null) :
        (data, ValidatingStates.InvalidError, CreateValidationException(error));
  }

  static (TData, ValidatingStates, Exception?) ValidateOutboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IValidatingData<TKey, TPayload> =>
    (data, ValidatingStates.Error, exception);

  internal static (TData, ValidatingStates, Exception?) ValidateOutboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken _ = default)
  where TServices : IValidatingServices<TKey, TPayload>
  where TData : IValidatingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ValidateOutboxMessageSuccess<TServices, TData, TKey, TPayload>,
      ValidateOutboxMessageError<TData, TKey, TPayload>
    );
}
