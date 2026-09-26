namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task transact_inbox_message__transaction_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<TransactingCapabilities<string, string, ISessionService>>();
    var session = Substitute.For<ISessionService>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var model = new object();
    var inputData = CreateInboxData(message, model: model);
    var expectedUpdate = new TransactingUpdate(InboxMessageStatus.Handled);
    capabilities.GetSession().Returns(session);
    session.CompleteAsync(default).Returns(Task.CompletedTask);
    capabilities.StoreDomainModelAsync(session, model, default).Returns(Task.CompletedTask);
    capabilities.UpdateInboxMessageAsync(session, GetInboxMessage<string, string>(inputData)!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.TransactInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(TransactingStates.Success);
    exception.ShouldBeNull();
    capabilities.GetSession.Received(1)();
    capabilities.StoreDomainModelAsync.Received(1)(session, model, default);
    capabilities.UpdateInboxMessageAsync.Received(1)(session, GetInboxMessage<string, string>(inputData)!, expectedUpdate, default);
    session.Received(1).CompleteAsync(default);
  }

  [TestMethod]
  public async Task transact_inbox_message__domain_model_missing__returns_error()
  {
    var capabilities = Fixture.Create<TransactingCapabilities<string, string, ISessionService>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = CreateInboxData(message);

    var (data, state, exception) = await InboxFuncs.TransactInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(TransactingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.GetSession.Received(0)();
  }
}
