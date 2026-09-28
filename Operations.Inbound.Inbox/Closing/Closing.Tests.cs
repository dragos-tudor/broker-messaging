namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task close_inbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<ClosingCapabilities>();
    var message = Fixture.Create<IInboxMessage>();
    var inputData = CreateInboxData(message);
    var expectedUpdate = new ClosingUpdate(InboxMessageStatus.Closed);
    capabilities.UpdateInboxMessageAsync(GetInboxMessage(inputData)!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateInboxMessageAsync.Received(1)(GetInboxMessage(inputData)!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task close_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ClosingCapabilities>();
    var inputData = CreateInboxData();

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.UpdateInboxMessageAsync.Received(0)(default!, default, default);
  }

  [TestMethod]
  public async Task close_inbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ClosingCapabilities>();
    var expectedException = new InvalidOperationException("close failed");
    var message = Fixture.Create<IInboxMessage>();
    var inputData = CreateInboxData(message);
    var expectedUpdate = new ClosingUpdate(InboxMessageStatus.Closed);
    capabilities.UpdateInboxMessageAsync(GetInboxMessage(inputData)!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateInboxMessageAsync.Received(1)(GetInboxMessage(inputData)!, expectedUpdate, default);
  }
}
