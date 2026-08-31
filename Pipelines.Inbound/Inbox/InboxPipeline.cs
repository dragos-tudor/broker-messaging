using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string InboxPipeline(string state) => state switch
  {
    ValidateInboxMessageSuccessState => InboxActions.Inserting,
    ValidateInboxMessageErrorState => InboxActions.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxActions.Unrecoverable,

    InsertInboxMessageSuccessState => InboxActions.Inserted,
    InsertInboxMessageErrorState => InboxActions.CheckingRetry,
    IdempotentInboxMessageState => InboxActions.Idempotent,

    CheckRetryInboxMessageExhaustedState => InboxActions.RetryExhausted,
    CheckRetryInboxMessageErrorState => InboxActions.CheckingRetry,

    UpsertRetryInboxMessageSuccessState => InboxActions.Deferring,
    UpsertRetryInboxMessageErrorState => InboxActions.UpsertingRetry,

    HandleInboxMessageSuccessState => InboxActions.Transacting,
    HandleInboxMessageDomainErrorState => InboxActions.Abandoning,
    HandleInboxMessageErrorState => InboxActions.Handling,

    TransactInboxMessageSuccessState => InboxActions.Transacted,
    TransactInboxMessageErrorState => InboxActions.Transacting,

    AbandonInboxMessageSuccessState => InboxActions.Converting,
    AbandonInboxMessageErrorState => InboxActions.Abandoning,

    ScheduleInboxMessageExhaustedState => InboxActions.Abandoning,
    ScheduleInboxMessageRetryState => InboxActions.Scheduled,
    ScheduleInboxMessageErrorState => InboxActions.Scheduling,

    ConvertInboxMessageSuccessState => InboxActions.Converted,
    ConvertInboxMessageErrorState => InboxActions.Unrecoverable,

    CloseInboxMessageSuccessState => InboxActions.Closed,
    CloseInboxMessageErrorState => InboxActions.Closing,

    _ => InboxActions.Unknown
  };
}

internal static class InboxActions
{
  private const string Scope = "Inbox";

  public const string Validating = $"{Scope}.{nameof(Validating)}";
  public const string Inserting = $"{Scope}.{nameof(Inserting)}";
  public const string Inserted = $"{Scope}.{nameof(Inserted)}";
  public const string Idempotent = $"{Scope}.{nameof(Idempotent)}";
  public const string CheckingRetry = $"{Scope}.{nameof(CheckingRetry)}";
  public const string UpsertingRetry = $"{Scope}.{nameof(UpsertingRetry)}";
  public const string RetryExhausted = $"{Scope}.{nameof(RetryExhausted)}";
  public const string Handling = $"{Scope}.{nameof(Handling)}";
  public const string Transacting = $"{Scope}.{nameof(Transacting)}";
  public const string Transacted = $"{Scope}.{nameof(Transacted)}";
  public const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  public const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  public const string Scheduled = $"{Scope}.{nameof(Scheduled)}";
  public const string Converting = $"{Scope}.{nameof(Converting)}";
  public const string Converted = $"{Scope}.{nameof(Converted)}";
  public const string Closing = $"{Scope}.{nameof(Closing)}";
  public const string Closed = $"{Scope}.{nameof(Closed)}";
  public const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
  public const string Deferring = $"{Scope}.{nameof(Deferring)}";
  public const string Unknown = $"{Scope}.{nameof(Unknown)}";
}