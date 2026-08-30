using static Operations.Inbound.Inbox.InboxStates;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> HandleInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IHandlingServices<TKey, TPayload>
  where TData : IHandlingData<TKey, TPayload>
  {
    try
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var options = services.GetInboxMessageOptions();

      var (model, domainError) = await services.HandleInboxMessageAsync(message, ct);
      if (domainError is not null)
      {
        data.PipelineError = domainError;
        return (data, HandleInboxMessageDomainErrorState, CreateDomainException(domainError));
      }

      data.Model = model;
      return (data, HandleInboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, HandleInboxMessageErrorState, exception);
    }
  }
}