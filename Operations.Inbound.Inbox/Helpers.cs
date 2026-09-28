
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static object RequireDomainModel(object? model) =>
    model ?? throw new InvalidOperationException("Inbound domain model is required.");

  static IInboxMessage RequireInboxMessage(
    IInboxMessage? message) =>
    message ?? throw new InvalidOperationException("Inbox message is required.");
}