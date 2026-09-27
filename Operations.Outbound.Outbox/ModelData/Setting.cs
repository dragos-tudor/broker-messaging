namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  internal static object? SetDomainModel(object?[] data, object? model) =>
    data[DomainModelIndex] = model;
}
