
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  const int DomainModelIndex = 5;

  public static object? GetDomainModel(object?[] data) =>
    data[DomainModelIndex];
}