using Inbox = Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDeadLetteringPipeline(DeadLetteringSignal[] path, DeadLetteringDecision end) =>
    RunDeadLetteringPipeline(path, end, new InboundPipelineConfig());

  static void RunDeadLetteringPipeline(
    DeadLetteringSignal[] path,
    DeadLetteringDecision end,
    InboundPipelineConfig config)
  {
    DeadLetteringSignal[] possibleSignals = [DeadLetteringEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var decision = AdvanceDeadLetteringPipeline(signal, config);
      if (path[^1].Value == signal.Value)
      {
        decision.ShouldBe(end);
        return;
      }

      possibleSignals = decision switch
      {
        DeadLetteringActions action => [.. GetDeadLetteringPossibleSignals(action)],
        _ => []
      };
    }
  }

  static IEnumerable<DeadLetteringSignal> GetDeadLetteringPossibleSignals(DeadLetteringActions action) =>
    action switch
    {
      DeadLetteringActions.Converting => [.. Enum.GetValues<Inbox.ConvertingStates>()],
      DeadLetteringActions.Inserting => [.. Enum.GetValues<DeadLetter.InsertingStates>()],
      DeadLetteringActions.Abandoning => [.. Enum.GetValues<Inbox.AbandoningStates>()],
      DeadLetteringActions.Closing => [.. Enum.GetValues<Inbox.ClosingStates>()],
      _ => throw new InvalidOperationException($"Invalid dead-lettering action {action}"),
    };
}

