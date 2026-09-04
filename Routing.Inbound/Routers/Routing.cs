
namespace Routing.Inbound;

partial class InboundFuncs
{
  internal static async Task<(List<string> States, List<string> Actions)>
    RunInboundPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
      TServices services,
      TData data,
      string initialAction,
      bool useBrokerPublisher = false,
      CancellationToken ct = default)
    where TServices : IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : InboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    var states = new List<string>();
    var actions = new List<string>();

    var currentAction = initialAction;
    actions.Add(currentAction);

    while (true)
    {
      var operation = GetInboundPipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(currentAction);
      if (operation is null) break;

      var (nextData, state, exception) = await operation(services, data, ct);
      data = nextData;
      states.Add(state);

      var localAction = GetInboundPipelineAction(state);
      if (localAction is null) break;
      actions.Add(localAction);

      var config = new InboundMappingConfig() {
        ShouldHandleMessage = data.InboxMessage?.Status == InboxMessageStatus.Processing,
        UseBrokerPublisher = useBrokerPublisher
      };
      var nextAction = MapInboundPipeline(localAction, config) ?? localAction;
      if (nextAction != localAction)
        actions.Add(nextAction);

      // If action did not change or maps to self without dispatchable operation, stop
      if (nextAction == currentAction)
        break;

      currentAction = nextAction;
    }

    return (states, actions);
  }

}