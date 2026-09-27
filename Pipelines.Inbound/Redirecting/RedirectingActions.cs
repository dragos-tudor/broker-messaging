namespace Pipelines.Inbound;

internal static class RedirectingActions
{
  const string Scope = $"{nameof(RedirectingActions)}";
  internal const string Converting = $"{Scope}.{nameof(Converting)}";
  internal const string Redirecting = $"{Scope}.{nameof(Redirecting)}";
  internal const string ConfirmingFinal = $"{Scope}.{nameof(ConfirmingFinal)}";
}
