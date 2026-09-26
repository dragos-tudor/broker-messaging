
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  public static object? SetDomainModel(object?[] objects, object? model) =>
    objects[DomainModelIndex] = model;
}