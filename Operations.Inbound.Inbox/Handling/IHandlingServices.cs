
namespace Operations.Inbound.Inbox;

public interface IHandlingServices<TKey, TPayload> :
  IInboxMessageHandlerService<TKey, TPayload>,
  IInboxMessageOptionsReaderService;