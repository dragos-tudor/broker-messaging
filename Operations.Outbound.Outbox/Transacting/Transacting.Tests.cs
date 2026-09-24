namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task transact_outbox_message__domain_model_missing__returns_error()
  {
    var capabilities = Fixture.Create<TransactingCapabilities<string, string, ISessionService>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new TransactingData<string, string>(message, default);

    var (data, state, exception) = await OutboxFuncs.TransactOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(TransactingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.GetSession.Received(0)();
  }

  [TestMethod]
  public async Task transact_outbox_message__transaction_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<TransactingCapabilities<string, string, ISessionService>>();
    var session = Substitute.For<ISessionService>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var model = Fixture.Create<object>();
    var inputData = new TransactingData<string, string>(message, model);
    capabilities.GetSession().Returns(session);
    capabilities.PersistDomainModelAsync(session, model, default).Returns(Task.CompletedTask);
    capabilities.InsertOutboxMessageAsync(session, inputData.OutboxMessage!, default).Returns(true);
    session.CompleteAsync(default).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.TransactOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(TransactingStates.Success);
    exception.ShouldBeNull();
    capabilities.GetSession.Received(1)();
    capabilities.PersistDomainModelAsync.Received(1)(session, model, default);
    capabilities.InsertOutboxMessageAsync.Received(1)(session, inputData.OutboxMessage!, default);
    session.Received(1).CompleteAsync(default);
  }
}
