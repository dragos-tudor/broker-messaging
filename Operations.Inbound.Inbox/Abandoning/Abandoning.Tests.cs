namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task abandon_inbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new AbandoningData<string, string>(message);
    var expectedUpdate = new AbandoningUpdate(
      InboxMessageStatus.Abandoned,
      message.LastError,
      message.FailureReason);
    capabilities.UpdateInboxMessageAsync(inputData.InboxMessage!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.AbandonInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateInboxMessageAsync.Received(1)(inputData.InboxMessage!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task abandon_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var inputData = new AbandoningData<string, string>(default);

    var (data, state, exception) = await InboxFuncs.AbandonInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.UpdateInboxMessageAsync.Received(0)(default!, default, default);
  }

  [TestMethod]
  public async Task abandon_inbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var expectedException = new InvalidOperationException("abandon failed");
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new AbandoningData<string, string>(message);
    var expectedUpdate = new AbandoningUpdate(
      InboxMessageStatus.Abandoned,
      message.LastError,
      message.FailureReason);
    capabilities.UpdateInboxMessageAsync(inputData.InboxMessage!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await InboxFuncs.AbandonInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateInboxMessageAsync.Received(1)(inputData.InboxMessage!, expectedUpdate, default);
  }
}
