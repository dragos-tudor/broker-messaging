namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public void validate_inbox_message__message_is_valid__returns_success()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = CreateInboxData(message);

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void validate_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities<string, string>>();
    var inputData = CreateInboxData<string, string>();

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void validate_inbox_message__message_is_invalid__returns_invalid_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities<string, string>>();
    var message = Fixture.Build<InboxMessage<string, string>>()
      .With(message => message.MessageId, Guid.Empty)
      .Create();
    var inputData = CreateInboxData(message);

    var (data, state, exception) = InboxFuncs.ValidateInboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.InvalidError);
    exception.ShouldNotBeNull();
  }
}
