namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class TransactingTestData : ITransactingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public object? Model { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task transacting__transact_inbox_message__success_when_session_transacted()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "transact-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var model = new object();

    var data = new TransactingTestData
    {
      InboxMessage = message,
      Model = model
    };

    var (resultData, state, exception) = await TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    session.Received(1).Dispose();
    await services.Received(1).TransactSessionAsync(session, Arg.Any<Func<IDisposable, Task>>(), Arg.Any<Func<IDisposable, Task>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task transacting__transact_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var data = new TransactingTestData
    {
      InboxMessage = null,
      Model = new object()
    };

    var (resultData, state, exception) = await TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }

  [TestMethod]
  public async Task transacting__transact_inbox_message__error_when_model_null()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "transact-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var data = new TransactingTestData
    {
      InboxMessage = message,
      Model = null
    };

    var (resultData, state, exception) = await TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox model is required.");
  }

  [TestMethod]
  public async Task transacting__transact_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);
    services.TransactSessionAsync(session, Arg.Any<Func<IDisposable, Task>>(), Arg.Any<Func<IDisposable, Task>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "transact-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var data = new TransactingTestData
    {
      InboxMessage = message,
      Model = new object()
    };

    var (resultData, state, exception) = await TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
    session.Received(1).Dispose();
  }

  [TestMethod]
  public async Task transacting__transact_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);
    var expectedException = new InvalidOperationException("Transaction aborted");
    services.TransactSessionAsync(session, Arg.Any<Func<IDisposable, Task>>(), Arg.Any<Func<IDisposable, Task>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "transact-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var data = new TransactingTestData
    {
      InboxMessage = message,
      Model = new object()
    };

    var (resultData, state, exception) = await TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Transaction aborted");
    session.Received(1).Dispose();
  }

  [TestMethod]
  public async Task transacting__transact_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "transact-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var data = new TransactingTestData
    {
      InboxMessage = message,
      Model = new object()
    };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await TransactInboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data, ct);

    await services.Received(1).TransactSessionAsync(session, Arg.Any<Func<IDisposable, Task>>(), Arg.Any<Func<IDisposable, Task>>(), ct);
  }
}

