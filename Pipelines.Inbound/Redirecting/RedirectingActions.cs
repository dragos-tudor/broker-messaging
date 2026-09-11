
namespace Pipelines.Inbound;

static class RedirectingActions
{
  internal const string Scope = "Redirecting";
  internal const string Converting = $"{Scope}.{nameof(Converting)}";
  internal const string Redirecting = $"{Scope}.{nameof(Redirecting)}";
  internal const string ConfirmingFinal = $"{Scope}.{nameof(ConfirmingFinal)}";
}