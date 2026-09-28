namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task dead_letter_inbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<DeadLetteringCapabilities>();
    var message = Fixture.Create<IInboxMessage>();
    var inputData = CreateInboxData(message);
    var expectedUpdate = new DeadLetteringUpdate(
      InboxMessageStatus.DeadLettering,
      message.LastError);
    capabilities.UpdateInboxMessageAsync(GetInboxMessage(inputData)!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.DeadLetterInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(DeadLetteringStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateInboxMessageAsync.Received(1)(GetInboxMessage(inputData)!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task dead_letter_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<DeadLetteringCapabilities>();
    var inputData = CreateInboxData();

    var (data, state, exception) = await InboxFuncs.DeadLetterInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(DeadLetteringStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.UpdateInboxMessageAsync.Received(0)(default!, default, default);
  }

  [TestMethod]
  public async Task dead_letter_inbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<DeadLetteringCapabilities>();
    var expectedException = new InvalidOperationException("dead lettering failed");
    var message = Fixture.Create<IInboxMessage>();
    var inputData = CreateInboxData(message);
    var expectedUpdate = new DeadLetteringUpdate(
      InboxMessageStatus.DeadLettering,
      message.LastError);
    capabilities.UpdateInboxMessageAsync(GetInboxMessage(inputData)!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await InboxFuncs.DeadLetterInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(DeadLetteringStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateInboxMessageAsync.Received(1)(GetInboxMessage(inputData)!, expectedUpdate, default);
  }
}
