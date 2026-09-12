namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task transact_outbox_message__domain_model_missing__returns_error()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };

    var (data, state, exception) = await OutboxFuncs.TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, OutboxData, string, string, IDisposable>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(TransactingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task transact_outbox_message__transaction_succeeds__returns_success()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>(), DomainModel = new object() };
    services.GetSession().Returns(session);
    services.TransactSessionAsync(Arg.Any<ITransactingServices<string, string, IDisposable>>(), Arg.Any<IDisposable>(), Arg.Any<(object model, IOutboxMessage<string, string> message)>(), Arg.Any<Func<ITransactingServices<string, string, IDisposable>, IDisposable, (object model, IOutboxMessage<string, string> message), CancellationToken, Task>>(), Arg.Any<Func<ITransactingServices<string, string, IDisposable>, IDisposable, (object model, IOutboxMessage<string, string> message), CancellationToken, Task>>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, OutboxData, string, string, IDisposable>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(TransactingSuccess);
    exception.ShouldBeNull();
  }
}
