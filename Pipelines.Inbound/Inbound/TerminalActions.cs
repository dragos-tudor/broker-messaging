
namespace Pipelines.Inbound;

static class TerminalActions
{
  const string Pipeline = "Inbound";
  internal const string Exit = $"{Pipeline}.{nameof(Exit)}";
  internal const string Unrecoverable = $"{Pipeline}.{nameof(Unrecoverable)}";
}