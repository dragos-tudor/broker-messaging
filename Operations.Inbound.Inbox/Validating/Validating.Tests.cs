namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void validate_inbox_message__message_is_valid__returns_success()
  {
    var services = Substitute.For<IValidatingServices>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage<IValidatingServices, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void validate_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IValidatingServices>();
    var inputData = new InboxData();

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage<IValidatingServices, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void validate_inbox_message__message_is_invalid__returns_invalid_error()
  {
    var services = Substitute.For<IValidatingServices>();
    var message = InboxData.CreateMessage() with { MessageId = Guid.Empty };
    var inputData = new InboxData { InboxMessage = message };

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage<IValidatingServices, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingStates.InvalidError);
    exception.ShouldNotBeNull();
  }
}
