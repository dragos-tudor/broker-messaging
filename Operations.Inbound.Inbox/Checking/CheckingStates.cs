
namespace Operations.Inbound.Inbox;

internal static class CheckingStates
{
  internal const string CheckRetryInboxMessageForInsertingExhaustedState = "CheckRetryInboxMessageForInsertingExhaustedState";
  internal const string CheckRetryInboxMessageForInsertingNotExhaustedState = "CheckRetryInboxMessageForInsertingNotExhaustedState";
  internal const string CheckRetryInboxMessageForInsertingErrorState = "CheckRetryInboxMessageForInsertingErrorState";
}