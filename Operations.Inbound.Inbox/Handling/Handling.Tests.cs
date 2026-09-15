namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task handle_inbox_message__handler_returns_model__returns_success_and_sets_model()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var model = new object();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<CancellationToken>()).Returns((model, (string?)null));

    var (data, state, exception) = await InboxFuncs.HandleInboxMessageAsync<IHandlingServices<string, string>, InboxData, string, string>(services, inputData);

    data.DomainModel.ShouldBeSameAs(model);
    state.ShouldBe(HandlingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task handle_inbox_message__handler_returns_domain_error__returns_domain_error()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<CancellationToken>()).Returns(((object?)null, "business failure"));

    var (data, state, exception) = await InboxFuncs.HandleInboxMessageAsync<IHandlingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(HandlingStates.DomainError);
    exception.ShouldBeOfType<DomainException>().Message.ShouldBe("business failure");
  }

  [TestMethod]
  public async Task handle_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.HandleInboxMessageAsync<IHandlingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(HandlingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
