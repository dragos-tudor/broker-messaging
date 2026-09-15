namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task dead_letter_inbox_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IDeadLetteringServices<string, string>>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<DeadLetteringUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.DeadLetterInboxMessageAsync<IDeadLetteringServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(DeadLetteringStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task dead_letter_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IDeadLetteringServices<string, string>>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.DeadLetterInboxMessageAsync<IDeadLetteringServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(DeadLetteringStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
