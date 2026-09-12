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
    state.ShouldBe(TransactingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
