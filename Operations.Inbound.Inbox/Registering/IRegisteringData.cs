
namespace Operations.Inbound.Inbox;

public interface IRegisteringRetryData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IRetryPlanProp,
  IPipelineErrorProp;
