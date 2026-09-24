using Funcs = Persistence.OutboxMessage.OutboxMessageFuncs;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (ValidatingData<TKey, TPayload>, ValidatingStates, Exception?)
    ValidateOutboxMessageSuccess<TKey, TPayload>(
      ValidatingCapabilities capabilities,
      ValidatingData<TKey, TPayload> data)
    {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var error = Funcs.ValidateOutboxMessage(message);
      return error is null
        ? (data, ValidatingStates.Success, null)
        : (data, ValidatingStates.InvalidError, Funcs.CreateValidationException(error));
    }

  static (ValidatingData<TKey, TPayload>, ValidatingStates, Exception?)
    ValidateOutboxMessageError<TKey, TPayload>(
      ValidatingData<TKey, TPayload> data,
      Exception exception) =>
    (data, ValidatingStates.Error, exception);

  internal static (ValidatingData<TKey, TPayload>, ValidatingStates, Exception?)
    ValidateOutboxMessage<TKey, TPayload>(
      ValidatingCapabilities capabilities,
      ValidatingData<TKey, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      ValidateOutboxMessageSuccess,
      ValidateOutboxMessageError);
}
