using Funcs = Persistence.OutboxMessage.OutboxMessageFuncs;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (TData, string, Exception?) ValidateOutboxMessageSuccess<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data)
  where TServices : IValidatingServices<TKey, TPayload>
  where TData : IValidatingData<TKey, TPayload>
  {
    var message = RequireOutboxMessage(data.OutboxMessage);

    var error = Funcs.ValidateOutboxMessage(message);
    return error is null?
        (data, ValidatingSuccess, null):
        (data, ValidatingInvalidError, CreateValidationException(error));
  }

  static (TData, string, Exception?) ValidateOutboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IValidatingData<TKey, TPayload> =>
    (data, ValidatingError, exception);

  internal static ValueTask<(TData, string, Exception?)> ValidateOutboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IValidatingServices<TKey, TPayload>
  where TData : IValidatingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ValidateOutboxMessageSuccess<TServices, TData, TKey, TPayload>,
      ValidateOutboxMessageError<TData, TKey, TPayload>
    );
}
