
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  const string Scope = nameof(OutboxStates);
  internal const string AbandoningSuccess = $"{Scope}.{nameof(AbandoningSuccess)}";
  internal const string AbandoningError = $"{Scope}.{nameof(AbandoningError)}";
}
