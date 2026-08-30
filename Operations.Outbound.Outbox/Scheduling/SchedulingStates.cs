
namespace Operations.Outbound.Outbox;

static partial class OutboxStates
{
  internal const string ScheduleOutboxMessageExhaustedState = "ScheduleOutboxMessageExhaustedState";
  internal const string ScheduleOutboxMessageRetryState = "ScheduleOutboxMessageRetryState";
  internal const string ScheduleOutboxMessageErrorState = "ScheduleOutboxMessageErrorState";
}