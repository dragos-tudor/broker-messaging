
namespace Pipelines.Inbound;

internal static class TerminalActions
{
  const string Scope = $"{nameof(TerminalActions)}";
  internal const string Exit = $"{Scope}{nameof(Exit)}";
  internal const string Unrecoverable = $"{Scope}{nameof(Unrecoverable)}";
  internal const string Unknown = $"{Scope}{nameof(Unknown)}";
}