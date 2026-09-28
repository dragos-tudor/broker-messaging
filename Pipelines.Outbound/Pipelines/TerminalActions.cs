namespace Pipelines.Outbound;

internal static class TerminalActions
{
  internal const string Scope = $"{nameof(TerminalActions)}";
  internal const string Exit = $"{Scope}.{nameof(Exit)}";
  internal const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
  internal const string Unknown = $"{Scope}.{nameof(Unknown)}";
}

partial class OutboundFuncs
{
  internal static bool IsTerminalAction(string decision) =>
    decision.StartsWith(TerminalActions.Scope, StringComparison.OrdinalIgnoreCase);
}