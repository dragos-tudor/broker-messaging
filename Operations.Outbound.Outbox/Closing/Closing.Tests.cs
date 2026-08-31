namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  sealed class ClosingTestData : IClosingData<string, string>
  {
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
  }

  [TestMethod]
  public async Task closing__close_outbox_message__success_when_message_updated()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-close-1",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ClosingTestData { OutboxMessage = message };

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await CloseOutboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    state.ShouldBe(CloseOutboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(OutboxMessageStatus.Published);
  }

  [TestMethod]
  public async Task closing__close_outbox_message__error_when_message_null()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var data = new ClosingTestData { OutboxMessage = null };

    var (resultData, state, exception) = await CloseOutboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    state.ShouldBe(CloseOutboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Outbox message is required.");
  }

  [TestMethod]
  public async Task closing__close_outbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-close-2",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    services.UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new ClosingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await CloseOutboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task closing__close_outbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-close-3",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var expectedException = new InvalidOperationException("Failed closing update");
    services.UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new ClosingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await CloseOutboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    state.ShouldBe(CloseOutboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task closing__close_outbox_message__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-close-4",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ClosingTestData { OutboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CloseOutboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), ct);
  }
}

