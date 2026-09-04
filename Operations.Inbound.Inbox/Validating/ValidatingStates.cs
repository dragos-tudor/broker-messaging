
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string ValidatingSuccess = $"{Scope}.{nameof(ValidatingSuccess)}";
  internal const string ValidatingError = $"{Scope}.{nameof(ValidatingError)}";
  internal const string ValidatingInvalidError = $"{Scope}.{nameof(ValidatingInvalidError)}";
}
