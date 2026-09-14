
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static (TData, ConvertingStates, Exception?) ConvertInboxMessageSuccess<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data)
  where TServices : IConvertingServices
  where TData : IConvertingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);

    var deadLetter = FromInboxMessage(message, services.GetUtcDateTime());
    SetDeadLetterMessage(data, deadLetter);

    return (data, ConvertingSuccess, null);
  }

  static (TData, ConvertingStates, Exception?) ConvertInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IConvertingData<TKey, TPayload> =>
    (data, ConvertingError, exception);

  internal static ValueTask<(TData, ConvertingStates, Exception?)> ConvertInboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IConvertingServices
  where TData : IConvertingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ConvertInboxMessageSuccess<TServices, TData, TKey, TPayload>,
      ConvertInboxMessageError<TData, TKey, TPayload>
    );
}
