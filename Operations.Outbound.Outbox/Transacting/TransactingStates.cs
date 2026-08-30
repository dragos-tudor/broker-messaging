
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string TransactOutboxMessageSuccessState = "TransactOutboxMessageSuccessState";
  internal const string TransactOutboxMessageErrorState = "TransactOutboxMessageErrorState";
  internal const string IdempotentOutboxMessageState = "IdempotentOutboxMessageState";
}