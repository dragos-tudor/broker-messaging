
namespace Operations.Inbound.Inbox;

public interface IDeadLetterMessageProp<TKey, TPayload> { DeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; } }

public interface IInboxMessageProp<TKey, TPayload> { InboxMessage<TKey, TPayload>? InboxMessage { get; set; } }

public interface IModelProp { object? Model { get; set; } }

public interface IPipelineErrorProp { string? PipelineError { get; set; } }

public interface IRetryPlanProp { RetryPlan? RetryPlan { get; set; } }


