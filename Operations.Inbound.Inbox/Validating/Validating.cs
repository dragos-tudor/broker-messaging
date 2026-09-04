
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ValidateInboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IValidatingServices
  where TData : IValidatingData<TKey, TPayload>
  {
    try {
      var message = RequireInboxMessage(data.InboxMessage);

      if (InboxMessageFuncs.ValidateInboxMessage(message) is IEnumerable<string> valErrors && valErrors.Any()) {
        data.InboxMessage = null;
        data.PipelineError = JoinValidationErrors(valErrors);
        return new ((data, ValidatingInvalidError, InboxMessageFuncs.CreateValidationException(data.PipelineError)));
      }

      return new ((data, ValidatingSuccess, null));
    }
    catch (Exception exception) {
      data.InboxMessage = null;
      data.PipelineError = exception.Message;
      return new ((data, ValidatingError, exception));
    }
  }
}
