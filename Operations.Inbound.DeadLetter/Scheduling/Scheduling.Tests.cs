namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  [DataRow(5, SchedulingStates.NotExhausted)]
  [DataRow(0, SchedulingStates.Exhausted)]
  public async Task schedule_dead_letter_message__retry_limit_varies__returns_matching_state(int maxRetries, Enum expectedState)
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };
    services.GetDeadLetterRetryOptions().Returns(CreateDeadLetterRetryOptions(maxRetries));
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, DeadLetterData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task schedule_dead_letter_message__update_throws__returns_error_with_exception()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var expectedException = new InvalidOperationException("schedule failed");
    services.GetDeadLetterRetryOptions().Returns(CreateDeadLetterRetryOptions());
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };

    var (data, state, exception) = await DeadLetterFuncs.ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, DeadLetterData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(SchedulingStates.Error);
    exception.ShouldBeSameAs(expectedException);
  }
}
