
namespace Pipelines.Inbound;

static class TerminalActions
{
  const string Scope = "Terminal";
  internal const string Exit = $"{Scope}.{nameof(Exit)}";
  internal const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
}