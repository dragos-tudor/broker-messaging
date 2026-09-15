namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task abandon_inbox_message__update_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var expectedException = new InvalidOperationException("abandon failed");
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<AbandoningUpdate>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };

    var (data, state, exception) = await InboxFuncs.AbandonInboxMessageAsync<IAbandoningServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(AbandoningStates.Error);
    exception.ShouldBeSameAs(expectedException);
  }
}
