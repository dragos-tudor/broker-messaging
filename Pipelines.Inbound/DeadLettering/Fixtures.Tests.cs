using Inbox = Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDeadLetteringPipeline(
    DeadLetteringSignal[] path,
    DeadLetteringTransition end,
    InboundPipelineConfig config = default)
  {
    DeadLetteringSignal[] possibleSignals = [DeadLetteringEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var transition = GetDeadLetteringTransition(signal, config);
      if (path[^1].Value == signal.Value)
      {
        transition.ShouldBe(end);
        return;
      }

      possibleSignals = transition switch
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

