namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  [DataRow(5, SchedulingStates.NotExhausted)]
  [DataRow(0, SchedulingStates.Exhausted)]
  public async Task schedule_inbox_message__retry_limit_varies__returns_matching_state(int maxRetries, string expectedState)
  {
    var capabilities = Fixture.Create<SchedulingCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    message.RetryCount = 0;
    var inputData = new SchedulingData<string, string>(message);
    var options = Fixture.Build<InboxRetryOptions>()
      .With(options => options.MaxRetryAttempts, maxRetries)
      .With(options => options.RetryBaseDelay, TimeSpan.Zero)
      .With(options => options.MaxRetryDelay, TimeSpan.Zero)
      .Create();
    var now = Fixture.Create<DateTime>();
    var nextRetryCount = IncrementInboxRetryCount(message.RetryCount);
    var expectedUpdate = new SchedulingUpdate(
      nextRetryCount,
      CalculateNextAttemptAt(nextRetryCount, now, options),
      GetInboxMessageStatus(nextRetryCount, options),
      message.LastError,
      message.FailureReason);
    capabilities.GetInboxRetryOptions().Returns(options);
    capabilities.GetUtcDateTime().Returns(now);
    capabilities.UpdateInboxMessageAsync(inputData.InboxMessage!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.ScheduleInboxMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
    capabilities.GetInboxRetryOptions.Received(1)();
    capabilities.GetUtcDateTime.Received(1)();
    capabilities.UpdateInboxMessageAsync.Received(1)(inputData.InboxMessage!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task schedule_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<SchedulingCapabilities<string, string>>();
    var inputData = new SchedulingData<string, string>(default);

    var (data, state, exception) = await InboxFuncs.ScheduleInboxMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(SchedulingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.GetInboxRetryOptions.Received(0)();
    capabilities.GetUtcDateTime.Received(0)();
    capabilities.UpdateInboxMessageAsync.Received(0)(default!, default, default);
  }
}
