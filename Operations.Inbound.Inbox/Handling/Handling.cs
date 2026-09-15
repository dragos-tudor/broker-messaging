
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, HandlingStates, Exception?)> HandleInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IHandlingServices<TKey, TPayload>
  where TData : IHandlingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);

    var (model, error) = await services.HandleInboxMessageAsync(message, ct);
    if (error is not null)
      return (data, HandlingStates.DomainError, CreateDomainException(error));

    SetDomainModel(data, model!);
    return (data, HandlingStates.Success, null);
  }

  static (TData, HandlingStates, Exception?) HandleInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IHandlingData<TKey, TPayload> =>
    (data, HandlingStates.Error, exception);

  internal static ValueTask<(TData, HandlingStates, Exception?)> HandleInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IHandlingServices<TKey, TPayload>
  where TData : IHandlingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      HandleInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      HandleInboxMessageError<TData, TKey, TPayload>,
      ct
    );
}
