namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [DataTestMethod]
  [DataRow(5, "DeadLetterStates.SchedulingNotExhausted")]
  [DataRow(0, "DeadLetterStates.SchedulingExhausted")]
  public async Task schedule_dead_letter_message__retry_limit_varies__returns_matching_state(int maxRetries, string expectedState)
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };
    services.GetDeadLetterMessageOptions().Returns(CreateDeadLetterMessageOptions(maxRetries));
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, DeadLetterData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }
}
