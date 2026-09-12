namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task close_inbox_message__update_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var expectedException = new InvalidOperationException("close failed");
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<ClosingUpdate>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync<IClosingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ClosingError);
    exception.ShouldBeSameAs(expectedException);
  }
}
