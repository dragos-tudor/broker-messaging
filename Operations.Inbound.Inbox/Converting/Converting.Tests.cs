namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task convert_inbox_message__message_exists__returns_success_and_sets_dead_letter()
  {
    var services = Substitute.For<IConvertingServices>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.GetUtcDateTime().Returns(DateTime.UtcNow);

    var (data, state, exception) = await InboxFuncs.ConvertInboxMessage<IConvertingServices, InboxData, string, string>(services, inputData);

    data.DeadLetterMessage.ShouldNotBeNull();
    state.ShouldBe(ConvertingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task convert_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IConvertingServices>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.ConvertInboxMessage<IConvertingServices, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ConvertingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
