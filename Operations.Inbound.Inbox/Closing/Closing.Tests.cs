namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task close_inbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new ClosingData<string, string>(message);
    var expectedUpdate = new ClosingUpdate(InboxMessageStatus.Closed);
    capabilities.UpdateInboxMessageAsync(inputData.InboxMessage!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateInboxMessageAsync.Received(1)(inputData.InboxMessage!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task close_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var inputData = new ClosingData<string, string>(default);

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.UpdateInboxMessageAsync.Received(0)(default!, default, default);
  }

  [TestMethod]
  public async Task close_inbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var expectedException = new InvalidOperationException("close failed");
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new ClosingData<string, string>(message);
    var expectedUpdate = new ClosingUpdate(InboxMessageStatus.Closed);
    capabilities.UpdateInboxMessageAsync(inputData.InboxMessage!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await InboxFuncs.CloseInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateInboxMessageAsync.Received(1)(inputData.InboxMessage!, expectedUpdate, default);
  }
}
