
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(HandlingData<TKey, TPayload>, HandlingStates, Exception?)>
    HandleInboxMessageSuccessAsync<TKey, TPayload>(
      HandlingCapabilities<TKey, TPayload> capabilities,
      HandlingData<TKey, TPayload>  data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(data.InboxMessage);

      var (model, error) = await capabilities.HandleInboxMessageAsync(message, ct);
      return error is not null?
        (data, HandlingStates.DomainError, CreateDomainException(error)) :
        (data with { Model = model }, HandlingStates.Success, null);
    }

  static (HandlingData<TKey, TPayload>, HandlingStates, Exception?)
    HandleInboxMessageError<TKey, TPayload>(
      HandlingData<TKey, TPayload> data,
      Exception exception) =>
    (data, HandlingStates.Error, exception);

  internal static Task<(HandlingData<TKey, TPayload>, HandlingStates, Exception?)>
    HandleInboxMessageAsync<TKey, TPayload>(
      HandlingCapabilities<TKey, TPayload> capabilities,
      HandlingData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      HandleInboxMessageSuccessAsync,
      HandleInboxMessageError,
      ct
    );
}
