using Funcs = Persistence.OutboxMessage.OutboxMessageFuncs;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (object?[], string, Exception?)
    ValidateOutboxMessageSuccess<TKey, TPayload>(
      ValidatingCapabilities<TKey, TPayload> capabilities,
      object?[] data)
  {
    var message = RequireOutboxMessage(GetOutboxMessage<TKey, TPayload>(data));
    var error = Funcs.ValidateOutboxMessage(message);

    return error is null ?
      (data, ValidatingStates.Success, null) :
      (data, ValidatingStates.InvalidError, CreateValidationException(error));
  }

  static (object?[], string, Exception?)
    ValidateOutboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ValidatingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ValidateOutboxMessage<TKey, TPayload>(
      ValidatingCapabilities<TKey, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ValidateOutboxMessageSuccess,
      ValidateOutboxMessageError);
}
