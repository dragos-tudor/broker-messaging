using static Operations.Outbound.Envelope.EnvelopeStates;
using static Operations.Outbound.Outbox.OutboxStates;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  [TestMethod]
  public void publishing__happy_path_with_broker_publisher__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, PublishingSuccess,
      ClosingSuccess, TerminalActions.Exit
    ];
    var config = new OutboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_error_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, PublishingError,
      SchedulingNotExhausted, TerminalActions.Exit
    ];
    var config = new OutboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_error_and_scheduling_exhausted__abandon_and_exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, PublishingError,
      SchedulingExhausted, AbandoningSuccess, TerminalActions.Exit
    ];
    var config = new OutboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_error_and_scheduling_error__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, PublishingError,
      SchedulingError, TerminalActions.Exit
    ];
    var config = new OutboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__publishing_success_and_closing_error__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, PublishingSuccess,
      ClosingError, TerminalActions.Exit
    ];
    var config = new OutboundPipelineConfig() { UseBrokerPublisher = true };
    RunPublishingPipeline(path, config);
  }

  [TestMethod]
  public void publishing__producing_enqueue__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, ProducingEnqueue,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_not_enqueue__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, ProducingNotEnqueue,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_error_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, ProducingError,
      SchedulingNotExhausted, TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_error_and_scheduling_exhausted__abandon_and_exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, ProducingError,
      SchedulingExhausted, AbandoningSuccess, TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__producing_error_and_scheduling_error__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingSuccess, ProducingError,
      SchedulingError, TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__mapping_error__abandon_and_exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingError, AbandoningSuccess,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }

  [TestMethod]
  public void publishing__mapping_error_and_abandoning_error__exit()
  {
    string[] path = [
      PipelinesTypes.Publishing, MappingError, AbandoningError,
      TerminalActions.Exit
    ];
    RunPublishingPipeline(path);
  }
}
