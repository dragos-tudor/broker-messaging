namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void convert_inbox_message__message_exists__returns_success_and_sets_dead_letter()
  {
    var capabilities = Fixture.Create<ConvertingCapabilities>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new ConvertingData<string, string>(message, default);
    var createdAt = Fixture.Create<DateTime>();
    capabilities.GetUtcDateTime().Returns(createdAt);

    var (data, state, exception) = InboxFuncs.ConvertInboxMessage(capabilities, inputData);

    data.DeadLetterMessage.ShouldNotBeNull();
    data.DeadLetterMessage!.FailureReason.ShouldBe(inputData.InboxMessage!.FailureReason);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();
    capabilities.GetUtcDateTime.Received(1)();
  }

  [TestMethod]
  public void convert_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ConvertingCapabilities>();
    var inputData = new ConvertingData<string, string>(default, default);

    var (data, state, exception) = InboxFuncs.ConvertInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConvertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.GetUtcDateTime.Received(0)();
  }
}
