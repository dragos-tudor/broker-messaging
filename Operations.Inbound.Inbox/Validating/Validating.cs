using Funcs = Persistence.InboxMessage.InboxMessageFuncs;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static (TData, ValidatingStates, Exception?) ValidateInboxMessageSuccess<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data)
  where TServices : IValidatingServices
  where TData : IValidatingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);

    var error = Funcs.ValidateInboxMessage(message);
    return error is not null?
      (data, ValidatingStates.InvalidError, Funcs.CreateValidationException(error)):
      (data, ValidatingStates.Success, null);
  }

  static (TData, ValidatingStates, Exception?) ValidateInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IValidatingData<TKey, TPayload> =>
    (data, ValidatingStates.Error, exception);

  internal static (TData, ValidatingStates, Exception?) ValidateInboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data)
  where TServices : IValidatingServices
  where TData : IValidatingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ValidateInboxMessageSuccess<TServices, TData, TKey, TPayload>,
      ValidateInboxMessageError<TData, TKey, TPayload>
    );
}
