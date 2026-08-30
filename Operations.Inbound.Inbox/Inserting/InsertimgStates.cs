
namespace Operations.Inbound.Inbox;

static partial class InboxStates
{
  internal const string InsertInboxMessageSuccessState = "InsertInboxMessageSuccessState";
  internal const string InsertInboxMessageErrorState = "InsertInboxMessageErrorState";
  internal const string IdempotentInboxMessageState = "IdempotentInboxMessageState";
  internal const string InsertInboxMessageExhaustedState = "InsertInboxMessageExhaustedState";
}