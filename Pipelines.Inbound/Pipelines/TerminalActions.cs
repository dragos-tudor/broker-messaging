
namespace Pipelines.Inbound;

internal static class TerminalActions
{
  internal const string Scope = $"{nameof(TerminalActions)}";
  internal const string Exit = $"{Scope}{nameof(Exit)}";
  internal const string Unrecoverable = $"{Scope}{nameof(Unrecoverable)}";
  internal const string Unknown = $"{Scope}{nameof(Unknown)}";
}

partial class InboundFuncs
{
  internal static bool IsTerminalAction(string decision) =>
    decision.StartsWith(TerminalActions.Scope, StringComparison.OrdinalIgnoreCase);
}