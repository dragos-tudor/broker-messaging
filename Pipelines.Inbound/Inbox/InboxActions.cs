
namespace Pipelines.Inbound;

static class InboxActions
{
  internal const string Scope = "Inbox";
  internal const string Validating = $"{Scope}.{nameof(Validating)}";
  internal const string Inserting = $"{Scope}.{nameof(Inserting)}";
  internal const string Inserted = $"{Scope}.{nameof(Inserted)}";
  internal const string Idempotent = $"{Scope}.{nameof(Idempotent)}";
  internal const string CheckingRetry = $"{Scope}.{nameof(CheckingRetry)}";
  internal const string RegisteringRetry = $"{Scope}.{nameof(RegisteringRetry)}";
  internal const string RetryExhausted = $"{Scope}.{nameof(RetryExhausted)}";
  internal const string Handling = $"{Scope}.{nameof(Handling)}";
  internal const string Transacting = $"{Scope}.{nameof(Transacting)}";
  internal const string Transacted = $"{Scope}.{nameof(Transacted)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  internal const string Scheduled = $"{Scope}.{nameof(Scheduled)}";
  internal const string Converting = $"{Scope}.{nameof(Converting)}";
  internal const string Converted = $"{Scope}.{nameof(Converted)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
  internal const string Closed = $"{Scope}.{nameof(Closed)}";
  internal const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
  internal const string Exit = $"{Scope}.{nameof(Exit)}";
}
