namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task validate_inbox_message__message_is_valid__returns_success()
  {
    var services = Substitute.For<IValidatingServices>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };

    var (data, state, exception) = await InboxFuncs.ValidateInboxMessage<IValidatingServices, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task validate_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IValidatingServices>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.ValidateInboxMessage<IValidatingServices, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
