using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod] public void publishing__happy_path_with_broker_publisher__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, PublishingStates.Success, ClosingStates.Success];
    RunPublishingPipeline(path, TerminalActions.Exit, new() { UseBrokerPublisher = true });
  }

  [TestMethod] public void publishing__publishing_error_and_scheduling_not_exhausted__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, PublishingStates.Error, SchedulingStates.NotExhausted];
    RunPublishingPipeline(path, TerminalActions.Exit, new() { UseBrokerPublisher = true });
  }

  [TestMethod] public void publishing__publishing_error_and_scheduling_exhausted__abandon_and_exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, PublishingStates.Error, SchedulingStates.Exhausted, AbandoningStates.Success];
    RunPublishingPipeline(path, TerminalActions.Exit, new() { UseBrokerPublisher = true });
  }

  [TestMethod] public void publishing__publishing_error_and_scheduling_error__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, PublishingStates.Error, SchedulingStates.Error];
    RunPublishingPipeline(path, TerminalActions.Exit, new() { UseBrokerPublisher = true });
  }

  [TestMethod] public void publishing__producing_enqueue__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, ProducingStates.Enqueue];
    RunPublishingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void publishing__producing_not_enqueue__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, ProducingStates.NotEnqueue];
    RunPublishingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void publishing__producing_error_and_scheduling_not_exhausted__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, ProducingStates.Error, SchedulingStates.NotExhausted];
    RunPublishingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void publishing__producing_error_and_scheduling_exhausted__abandon_and_exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, ProducingStates.Error, SchedulingStates.Exhausted, AbandoningStates.Success];
    RunPublishingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void publishing__mapping_error__abandon_and_exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Error, AbandoningStates.Success];
    RunPublishingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void publishing__mapping_error_and_abandoning_error__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Error, AbandoningStates.Error];
    RunPublishingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void publishing__closing_error__exit()
  {
    PublishingInput[] path = [PublishingEntry.Start, MappingStates.Success, PublishingStates.Success, ClosingStates.Error];
    RunPublishingPipeline(path, TerminalActions.Exit, new() { UseBrokerPublisher = true });
  }
}
