namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task close_inbox_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = InboxData.CreateMessage();
    var inputData = new InboxData { InboxMessage = message };
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<ClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync<IClosingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ClosingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task close_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync<IClosingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
