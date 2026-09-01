
namespace Pipelines.Inbound;

internal static class DeadLetterActions
{
  internal const string Scope = "DeadLetter";
  internal const string Inserting = $"{Scope}.{nameof(Inserting)}";
  internal const string Mapping = $"{Scope}.{nameof(Mapping)}";
  internal const string Mapped = $"{Scope}.{nameof(Mapped)}";
  internal const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  internal const string Scheduled = $"{Scope}.{nameof(Scheduled)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Abandoned = $"{Scope}.{nameof(Abandoned)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
  internal const string Closed = $"{Scope}.{nameof(Closed)}";
  internal const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
}