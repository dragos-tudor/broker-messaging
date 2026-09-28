namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void convert_inbox_message__message_exists__returns_success_and_sets_dead_letter()
  {
    var capabilities = Fixture.Create<ConvertingCapabilities>();
    var message = Fixture.Create<IInboxMessage>();
    var deadLetterMessage = Fixture.Create<IDeadLetterMessage>();
    var inputData = CreateInboxData(message);
    var createdAt = Fixture.Create<DateTime>();
    capabilities.GetUtcDateTime().Returns(createdAt);
    capabilities.FromInboxMessage(message, createdAt).Returns(deadLetterMessage);

    var (data, state, exception) = InboxFuncs.ConvertInboxMessage(capabilities, inputData);

    GetDeadLetterMessage(data).ShouldBe(deadLetterMessage);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();
    capabilities.GetUtcDateTime.Received(1)();
  }

  [TestMethod]
  public void convert_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ConvertingCapabilities>();
    var inputData = CreateInboxData();

    var (data, state, exception) = InboxFuncs.ConvertInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConvertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.GetUtcDateTime.Received(0)();
  }
}
