
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string ValidatingSuccess = $"{Scope}.{nameof(ValidatingSuccess)}";
  internal const string ValidatingError = $"{Scope}.{nameof(ValidatingError)}";
  internal const string ValidatingInvalidError = $"{Scope}.{nameof(ValidatingInvalidError)}";
}
