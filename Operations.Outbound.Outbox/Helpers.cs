
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static object RequireDomainModel(object? model) =>
    model ?? throw new InvalidOperationException("Outbound domain model is required.");

  static IOutboxMessage RequireOutboxMessage(IOutboxMessage? message) =>
    message ?? throw new InvalidOperationException("Outbox message is required");
}
