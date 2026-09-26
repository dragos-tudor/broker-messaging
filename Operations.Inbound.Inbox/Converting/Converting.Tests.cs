namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void convert_inbox_message__message_exists__returns_success_and_sets_dead_letter()
  {
    var capabilities = Fixture.Create<ConvertingCapabilities>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = CreateInboxData(message);
    var createdAt = Fixture.Create<DateTime>();
    capabilities.GetUtcDateTime().Returns(createdAt);

    var (data, state, exception) = InboxFuncs.ConvertInboxMessage<string, string>(capabilities, inputData);

    GetDeadLetterMessage<string, string>(data).ShouldNotBeNull();
    GetDeadLetterMessage<string, string>(data)!.FailureReason.ShouldBe(GetInboxMessage<string, string>(inputData)!.FailureReason);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();
    capabilities.GetUtcDateTime.Received(1)();
  }

  [TestMethod]
  public void convert_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ConvertingCapabilities>();
    var inputData = CreateInboxData<string, string>();

    var (data, state, exception) = InboxFuncs.ConvertInboxMessage<string, string>(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConvertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.GetUtcDateTime.Received(0)();
  }
}
