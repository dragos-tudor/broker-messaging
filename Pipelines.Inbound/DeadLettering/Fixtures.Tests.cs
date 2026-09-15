using Inbox = Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDeadLetteringPipeline(
    DeadLetteringInput[] path,
    DeadLetteringContinuation end,
    InboundPipelineConfig config = default)
  {
    DeadLetteringInput[] possibleInputs = [DeadLetteringEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input, $"{input} is not valid. Expected one of: {string.Join(", ", possibleInputs)}");

      var continuation = GetDeadLetteringContinuation(input, config);
      if (path[^1].Value == input.Value)
      {
        continuation.ShouldBe(end);
        return;
      }

      possibleInputs = continuation switch
      {
        DeadLetteringActions action => [.. GetDeadLetteringPossibleInputs(action)],
        _ => []
      };
    }
  }

  static IEnumerable<DeadLetteringInput> GetDeadLetteringPossibleInputs(DeadLetteringActions action) =>
    action switch
    {
      DeadLetteringActions.Converting => [.. Enum.GetValues<Inbox.ConvertingStates>()],
      DeadLetteringActions.Inserting => [.. Enum.GetValues<DeadLetter.InsertingStates>()],
      DeadLetteringActions.Abandoning => [.. Enum.GetValues<Inbox.AbandoningStates>()],
      DeadLetteringActions.Closing => [.. Enum.GetValues<Inbox.ClosingStates>()],
      _ => throw new InvalidOperationException($"Invalid dead-lettering action {action}"),
    };
}
