using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ValidateOutboxMessage<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IValidatingServices<TKey, TPayload>
  where TData : IValidatingData<TKey, TPayload>
  {
    try {
      var message = RequireOutboxMessage(data.OutboxMessage);

      if (OutboxMessageFuncs.ValidateOutboxMessage(message) is IEnumerable<string> errors && errors.Any()) {
        data.OutboxMessage = null;
        return new ((data, ValidatingInvalidError, CreateValidationException(errors)));
      }

      return new ((data, ValidatingSuccess, null));
    }
    catch (Exception exception) {
      data.OutboxMessage = null;
      return new ((data, ValidatingError, exception));
    }
  }
}
