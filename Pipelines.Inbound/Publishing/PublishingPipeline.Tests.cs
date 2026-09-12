using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void publishing__happy_path_with_broker_publisher__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, PublishingSuccess,
      ClosingSuccess, TerminalActions.Exit
    ];
    var config = new InboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_error_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, PublishingError,
      SchedulingNotExhausted, TerminalActions.Exit
    ];
    var config = new InboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_error_and_scheduling_exhausted__abandon_and_exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, PublishingError,
      SchedulingExhausted, AbandoningSuccess, TerminalActions.Exit
    ];
    var config = new InboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_error_and_scheduling_error__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, PublishingError,
      SchedulingError, TerminalActions.Exit
    ];
    var config = new InboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__producing_enqueue__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, ProducingEnqueue,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_not_enqueue__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, ProducingNotEnqueue,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_error_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, ProducingError,
      SchedulingNotExhausted, TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_error_and_scheduling_exhausted__abandon_and_exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, ProducingError,
      SchedulingExhausted, AbandoningSuccess, TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__mapping_error__abandon_and_exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingError, AbandoningSuccess,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__mapping_error_and_abandoning_error__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingError, AbandoningError,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__closing_error__exit()
  {
    string[] path = [
      PipelineTypes.Publishing, MappingSuccess, PublishingSuccess,
      ClosingError, TerminalActions.Exit
    ];
    var config = new InboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }
}
