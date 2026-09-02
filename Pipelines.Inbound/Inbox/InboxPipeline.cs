using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetInboxPipelineAction(string state) => state switch
  {
    ValidateInboxMessageSuccessState => InboxActions.Inserting,
    ValidateInboxMessageErrorState => InboxActions.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxActions.Unrecoverable,

    InsertInboxMessageSuccessState => InboxActions.Inserted,
    InsertInboxMessageErrorState => InboxActions.CheckingRetry,
    IdempotentInboxMessageState => InboxActions.Idempotent,

    CheckRetryInboxMessageExhaustedState => InboxActions.RetryExhausted,
    CheckRetryInboxMessageNotExhaustedState => InboxActions.RegisteringRetry,
    CheckRetryInboxMessageErrorState => InboxActions.CheckingRetry,

    RegisterRetryInboxMessageSuccessState => InboxActions.Exit,
    RegisterRetryInboxMessageErrorState => InboxActions.RegisteringRetry,

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

    _ => default
  };
}
