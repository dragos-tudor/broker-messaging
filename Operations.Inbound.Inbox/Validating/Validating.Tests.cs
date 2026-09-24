namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void validate_inbox_message__message_is_valid__returns_success()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new ValidatingData<string, string>(message);

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void validate_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var inputData = new ValidatingData<string, string>(default);

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void validate_inbox_message__message_is_invalid__returns_invalid_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Build<InboxMessage<string, string>>()
      .With(message => message.MessageId, Guid.Empty)
      .Create();
    var inputData = new ValidatingData<string, string>(message);

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.InvalidError);
    exception.ShouldNotBeNull();
  }
}
