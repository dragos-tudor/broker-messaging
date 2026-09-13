
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  static int GetInboxMessagePayloadLength<TPayload>(TPayload payload) =>
    payload switch {
      null => 0,
      string s => s.Length,
      byte[] b => b.Length,
      _ => throw new NotSupportedException(
        $"Cannot measure payload of type {typeof(TPayload).Name}. " +
        "Expected string or a byte array type.")
    };
}