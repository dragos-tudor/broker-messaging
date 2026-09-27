
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  public static object? SetDomainModel(object?[] data, object? model) =>
    data[DomainModelIndex] = model;
}