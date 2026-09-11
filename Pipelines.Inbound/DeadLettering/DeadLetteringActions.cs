
namespace Pipelines.Inbound;

static class DeadLetteringActions
{
  internal const string Scope = "DeadLettering";
  internal const string Converting = $"{Scope}.{nameof(Converting)}";
  internal const string Inserting = $"{Scope}.{nameof(Inserting)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
}
