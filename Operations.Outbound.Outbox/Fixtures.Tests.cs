
namespace Operations.Outbound.Outbox;

partial class OutboxTests
{
  static object?[] CreateOutboxData(
    IOutboxMessage? message,
    object? model = null)
  {
    object?[] data = new object?[4];
    if (message is not null)
      SetOutboxMessage(data, message);
    if (model is not null)
      SetDomainModel(data, model);
    return data;
  }

  static object?[] CreateOutboxMappingData(
    IOutboxMessage? message,
    IEnvelope? envelope = null)
  {
    var data = CreateOutboxData(message);
    if (envelope is not null)
      SetEnvelope(data, envelope);
    return data;
  }
}
