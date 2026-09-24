namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task handle_inbox_message__handler_returns_model__returns_success_and_sets_model()
  {
    var capabilities = Fixture.Create<HandlingCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var model = new object();
    var inputData = new HandlingData<string, string>(message);
    capabilities.HandleInboxMessageAsync(inputData.InboxMessage!, default).Returns((model, null));

    var (data, state, exception) = await InboxFuncs.HandleInboxMessageAsync(capabilities, inputData);

    data.Model.ShouldBeSameAs(model);
    state.ShouldBe(HandlingStates.Success);
    exception.ShouldBeNull();
    capabilities.HandleInboxMessageAsync.Received(1)(inputData.InboxMessage!, default);
  }

  [TestMethod]
  public async Task handle_inbox_message__handler_returns_domain_error__returns_domain_error()
  {
    var capabilities = Fixture.Create<HandlingCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = new HandlingData<string, string>(message);
    capabilities.HandleInboxMessageAsync(inputData.InboxMessage!, default).Returns((null, "business failure"));

    var (data, state, exception) = await InboxFuncs.HandleInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(HandlingStates.DomainError);
    exception.ShouldBeOfType<DomainException>().Message.ShouldBe("business failure");
    capabilities.HandleInboxMessageAsync.Received(1)(inputData.InboxMessage!, default);
  }

  [TestMethod]
  public async Task handle_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<HandlingCapabilities<string, string>>();
    var inputData = new HandlingData<string, string>(default);

    var (data, state, exception) = await InboxFuncs.HandleInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(HandlingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.HandleInboxMessageAsync.Received(0)(default!, default);
  }
}
