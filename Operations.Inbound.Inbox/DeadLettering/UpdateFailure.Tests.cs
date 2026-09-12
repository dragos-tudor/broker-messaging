namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task dead_letter_inbox_message__update_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IDeadLetteringServices<string, string>>();
    var expectedException = new InvalidOperationException("dead lettering failed");
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<DeadLetteringUpdate>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };

    var (data, state, exception) = await InboxFuncs.DeadLetterInboxMessageAsync<IDeadLetteringServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(DeadLetteringError);
    exception.ShouldBeSameAs(expectedException);
  }
}
