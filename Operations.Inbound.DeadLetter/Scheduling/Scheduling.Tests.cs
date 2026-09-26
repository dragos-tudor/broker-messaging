namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  [DataRow(5, SchedulingStates.NotExhausted)]
  [DataRow(0, SchedulingStates.Exhausted)]
  public async Task schedule_dead_letter_message__retry_limit_varies__returns_matching_state(int maxRetries, string expectedState)
  {
    var capabilities = Fixture.Create<SchedulingCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    message.RetryCount = 0;
    var inputData = new SchedulingData<string, string>(message);
    var options = Fixture.Build<DeadLetterRetryOptions>()
      .With(options => options.MaxRetryAttempts, maxRetries)
      .With(options => options.RetryBaseDelay, TimeSpan.Zero)
      .With(options => options.MaxRetryDelay, TimeSpan.Zero)
      .Create();
    var now = Fixture.Create<DateTime>();
    var nextRetryCount = IncrementDeadLetterRetryCount(message.RetryCount);
    var expectedUpdate = new SchedulingUpdate(
      nextRetryCount,
      CalculateNextAttemptAt(nextRetryCount, now, options),
      GetDeadLetterMessageStatus(nextRetryCount, options),
      message.LastError);
    capabilities.GetDeadLetterRetryOptions().Returns(options);
    capabilities.GetUtcDateTime().Returns(now);
    capabilities.UpdateDeadLetterMessageAsync(inputData.Message!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.ScheduleDeadLetterMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
    capabilities.GetDeadLetterRetryOptions.Received(1)();
    capabilities.GetUtcDateTime.Received(1)();
    capabilities.UpdateDeadLetterMessageAsync.Received(1)(inputData.Message!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task schedule_dead_letter_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<SchedulingCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    message.RetryCount = 0;
    var inputData = new SchedulingData<string, string>(message);
    var options = Fixture.Build<DeadLetterRetryOptions>()
      .With(options => options.MaxRetryAttempts, 5)
      .With(options => options.RetryBaseDelay, TimeSpan.Zero)
      .With(options => options.MaxRetryDelay, TimeSpan.Zero)
      .Create();
    var now = Fixture.Create<DateTime>();
    var nextRetryCount = IncrementDeadLetterRetryCount(message.RetryCount);
    var expectedUpdate = new SchedulingUpdate(
      nextRetryCount,
      CalculateNextAttemptAt(nextRetryCount, now, options),
      GetDeadLetterMessageStatus(nextRetryCount, options),
      message.LastError);
    var expectedException = new InvalidOperationException("schedule failed");
    capabilities.GetDeadLetterRetryOptions().Returns(options);
    capabilities.GetUtcDateTime().Returns(now);
    capabilities.UpdateDeadLetterMessageAsync(inputData.Message!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await DeadLetterFuncs.ScheduleDeadLetterMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(SchedulingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateDeadLetterMessageAsync.Received(1)(inputData.Message!, expectedUpdate, default);
  }
}
