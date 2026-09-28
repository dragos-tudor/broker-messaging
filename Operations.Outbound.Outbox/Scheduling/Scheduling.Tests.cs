namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  [DataRow(5, SchedulingStates.NotExhausted)]
  [DataRow(0, SchedulingStates.Exhausted)]
  public async Task schedule_outbox_message__retry_limit_varies__returns_matching_state(int maxRetries, string expectedState)
  {
    var capabilities = Fixture.Create<SchedulingCapabilities>();
    var message = Fixture.Create<IOutboxMessage>();
    message.RetryCount = 0;
    var inputData = CreateOutboxData(message);
    var options = Fixture.Build<OutboxRetryOptions>()
      .With(options => options.MaxRetryAttempts, maxRetries)
      .With(options => options.RetryBaseDelay, TimeSpan.Zero)
      .With(options => options.MaxRetryDelay, TimeSpan.Zero)
      .Create();
    var now = Fixture.Create<DateTime>();
    var nextRetryCount = IncrementOutboxRetryCount(message.RetryCount);
    var expectedUpdate = new SchedulingUpdate(nextRetryCount, CalculateNextAttemptAt(nextRetryCount, now, options), GetOutboxMessageStatus(nextRetryCount, options), message.LastError);
    capabilities.GetOutboxRetryOptions().Returns(options);
    capabilities.GetUtcDateTime().Returns(now);
    capabilities.UpdateOutboxMessageAsync(GetOutboxMessage(inputData)!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await ScheduleOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
    capabilities.GetOutboxRetryOptions.Received(1)();
    capabilities.GetUtcDateTime.Received(1)();
    capabilities.UpdateOutboxMessageAsync.Received(1)(GetOutboxMessage(inputData)!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task schedule_outbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<SchedulingCapabilities>();
    var message = Fixture.Create<IOutboxMessage>();
    message.RetryCount = 0;
    var inputData = CreateOutboxData(message);
    var options = Fixture.Build<OutboxRetryOptions>()
      .With(options => options.MaxRetryAttempts, 5)
      .With(options => options.RetryBaseDelay, TimeSpan.Zero)
      .With(options => options.MaxRetryDelay, TimeSpan.Zero)
      .Create();
    var now = Fixture.Create<DateTime>();
    var nextRetryCount = IncrementOutboxRetryCount(message.RetryCount);
    var expectedUpdate = new SchedulingUpdate(nextRetryCount, CalculateNextAttemptAt(nextRetryCount, now, options), GetOutboxMessageStatus(nextRetryCount, options), message.LastError);
    var expectedException = new InvalidOperationException("schedule failed");
    capabilities.GetOutboxRetryOptions().Returns(options);
    capabilities.GetUtcDateTime().Returns(now);
    capabilities.UpdateOutboxMessageAsync(GetOutboxMessage(inputData)!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await ScheduleOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(SchedulingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateOutboxMessageAsync.Received(1)(GetOutboxMessage(inputData)!, expectedUpdate, default);
  }
}
