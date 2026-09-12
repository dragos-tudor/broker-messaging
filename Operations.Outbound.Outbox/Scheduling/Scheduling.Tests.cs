namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  [DataRow(5, "OutboxStates.SchedulingNotExhausted")]
  [DataRow(0, "OutboxStates.SchedulingExhausted")]
  public async Task schedule_outbox_message__retry_limit_varies__returns_matching_state(int maxRetries, string expectedState)
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };
    services.GetOutboxMessageOptions().Returns(CreateOutboxMessageOptions(maxRetries));
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateOutboxMessageAsync(Arg.Any<IOutboxMessage<string, string>>(), Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, OutboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task schedule_outbox_message__update_throws__returns_error_with_exception()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var expectedException = new InvalidOperationException("schedule failed");
    services.GetOutboxMessageOptions().Returns(CreateOutboxMessageOptions());
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateOutboxMessageAsync(Arg.Any<IOutboxMessage<string, string>>(), Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };

    var (data, state, exception) = await OutboxFuncs.ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, OutboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(SchedulingError);
    exception.ShouldBeSameAs(expectedException);
  }
}
