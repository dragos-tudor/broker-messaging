namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task abandon_inbox_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.AbandonInboxMessageAsync<IAbandoningServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(AbandoningSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task abandon_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.AbandonInboxMessageAsync<IAbandoningServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(AbandoningError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
