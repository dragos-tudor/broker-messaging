namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  sealed class TransactingTestData : ITransactingData<string, string>
  {
    public object Model { get; init; } = new();
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
  }

  [TestMethod]
  public async Task transacting__transact_outbox_message__success_when_session_transacted()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);

    services.TransactSessionAsync(
      session,
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<CancellationToken>())
      .Returns(async callInfo =>
      {
        var action1 = callInfo.ArgAt<Func<IDisposable, Task>>(1);
        var action2 = callInfo.ArgAt<Func<IDisposable, Task>>(2);
        await action1(session);
        await action2(session);
      });

    var model = new object();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-tx-1",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new TransactingTestData
    {
      Model = model,
      OutboxMessage = message
    };

    var (resultData, state, exception) = await TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);

    session.Received(1).Dispose();
    await services.Received(1).PersistOutboxModelAsync(session, model);
    await services.Received(1).InsertOutboxMessageAsync(
      session,
      Arg.Is<OutboxMessage<string, string>>(m => m.Status == OutboxMessageStatus.Processing),
      Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task transacting__transact_outbox_message__error_when_message_null()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var data = new TransactingTestData
    {
      Model = new object(),
      OutboxMessage = null
    };

    var (resultData, state, exception) = await TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Outbox message is required.");
  }

  [TestMethod]
  public async Task transacting__transact_outbox_message__error_when_model_null()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-tx-null-model",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new TransactingTestData
    {
      Model = null!,
      OutboxMessage = message
    };

    var (resultData, state, exception) = await TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Model is required.");
  }

  [TestMethod]
  public async Task transacting__transact_outbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);

    services.TransactSessionAsync(
      session,
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-tx-cancel",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new TransactingTestData
    {
      Model = new object(),
      OutboxMessage = message
    };

    var (resultData, state, exception) = await TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
    session.Received(1).Dispose();
  }

  [TestMethod]
  public async Task transacting__transact_outbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);

    var expectedException = new InvalidOperationException("Transaction commit failed");
    services.TransactSessionAsync(
      session,
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-tx-err",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new TransactingTestData
    {
      Model = new object(),
      OutboxMessage = message
    };

    var (resultData, state, exception) = await TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data);

    state.ShouldBe(TransactingError);
    exception.ShouldBeSameAs(expectedException);
    session.Received(1).Dispose();
  }

  [TestMethod]
  public async Task transacting__transact_outbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-tx-token",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new TransactingTestData
    {
      Model = new object(),
      OutboxMessage = message
    };

    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, TransactingTestData, string, string, IDisposable>(services, data, ct);

    await services.Received(1).TransactSessionAsync(
      session,
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<Func<IDisposable, Task>>(),
      ct);
  }
}
