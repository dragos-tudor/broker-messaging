
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    HandleInboxMessageSuccessAsync(
      HandlingCapabilities capabilities,
      object?[]  data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));

      var (model, error) = await capabilities.HandleInboxMessageAsync(message, ct);
      if (error is not null)
        return (data, HandlingStates.DomainError, CreateDomainException(error));

      SetDomainModel(data, model);
      return (data, HandlingStates.Success, null);
    }

  static (object?[], string, Exception?)
    HandleInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, HandlingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    HandleInboxMessageAsync(
      HandlingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      HandleInboxMessageSuccessAsync,
      HandleInboxMessageError,
      ct
    );
}
