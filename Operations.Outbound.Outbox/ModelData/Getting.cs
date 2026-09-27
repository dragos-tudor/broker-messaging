namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  const int DomainModelIndex = 2;

  internal static object? GetDomainModel(object?[] data) => data[DomainModelIndex];
}
