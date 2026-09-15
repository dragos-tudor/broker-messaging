namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  [DataRow(5, SchedulingStates.NotExhausted)]
  [DataRow(0, SchedulingStates.Exhausted)]
  public async Task schedule_inbox_message__retry_limit_varies__returns_matching_state(int maxRetries, Enum expectedState)
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.GetInboxMessageOptions().Returns(CreateInboxMessageOptions(maxRetries));
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.ScheduleInboxMessageAsync<ISchedulingServices<string, string>, InboxData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task schedule_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.ScheduleInboxMessageAsync<ISchedulingServices<string, string>, InboxData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(SchedulingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
