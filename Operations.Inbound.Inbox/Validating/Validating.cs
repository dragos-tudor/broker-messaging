using Funcs = Persistence.InboxMessage.InboxMessageFuncs;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (TData, string, Exception?) ValidateInboxMessageSuccess<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data)
  where TServices : IValidatingServices
  where TData : IValidatingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);

    var error = Funcs.ValidateInboxMessage(message);
    return error is not null?
      (data, ValidatingInvalidError, Funcs.CreateValidationException(error)):
      (data, ValidatingSuccess, null);
  }

  static (TData, string, Exception?) ValidateInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IValidatingData<TKey, TPayload> =>
    (data, ValidatingError, exception);

  internal static ValueTask<(TData, string, Exception?)> ValidateInboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IValidatingServices
  where TData : IValidatingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ValidateInboxMessageSuccess<TServices, TData, TKey, TPayload>,
      ValidateInboxMessageError<TData, TKey, TPayload>
    );
}
