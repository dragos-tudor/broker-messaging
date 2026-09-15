namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  public async Task transact_inbox_message__domain_model_missing__returns_error()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };

    var (data, state, exception) = await InboxFuncs.TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, InboxData, string, string, IDisposable>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(TransactingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task transact_inbox_message__transaction_succeeds__returns_success()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage(), DomainModel = new object() };
    services.GetSession().Returns(session);
    services.TransactSessionAsync(Arg.Any<ITransactingServices<string, string, IDisposable>>(), Arg.Any<IDisposable>(), Arg.Any<(object model, IInboxMessage<string, string> message)>(), Arg.Any<Func<ITransactingServices<string, string, IDisposable>, IDisposable, (object model, IInboxMessage<string, string> message), CancellationToken, Task>>(), Arg.Any<Func<ITransactingServices<string, string, IDisposable>, IDisposable, (object model, IInboxMessage<string, string> message), CancellationToken, Task>>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await InboxFuncs.TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, InboxData, string, string, IDisposable>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(TransactingStates.Success);
    exception.ShouldBeNull();
  }
}
