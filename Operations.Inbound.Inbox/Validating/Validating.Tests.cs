namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void validate_inbox_message__message_is_valid__returns_success()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Create<IInboxMessage>();
    var inputData = CreateInboxData(message);

    capabilities.ValidateInboxMessage(message).Returns(default(string));
    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void validate_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var inputData = CreateInboxData();

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void validate_inbox_message__message_is_invalid__returns_invalid_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Create<IInboxMessage>();
    var inputData = CreateInboxData(message);

    capabilities.ValidateInboxMessage(message).Returns("invalid");
    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.InvalidError);
    exception.ShouldNotBeNull();
  }
}
