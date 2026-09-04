namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  sealed class AbandoningTestData : IAbandoningData<string, string>
  {
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task abandoning__abandon_outbox_message__success_when_message_updated()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-key-1",
      Payload = "payload-1",
      CreatedAt = DateTime.UtcNow
    };

    var data = new AbandoningTestData
    {
      OutboxMessage = message,
      PipelineError = "Abandoning error"
    };

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await AbandonOutboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandoningSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(OutboxMessageStatus.Abandoned);
    updated.LastError.ShouldBe("Abandoning error");
  }

  [TestMethod]
  public async Task abandoning__abandon_outbox_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-key-2",
      Payload = "payload-2",
      CreatedAt = DateTime.UtcNow
    };

    var data = new AbandoningTestData
    {
      OutboxMessage = message,
      PipelineError = null!
    };

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await AbandonOutboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandoningSuccess);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(OutboxMessageStatus.Abandoned);
    updated.LastError.ShouldBe("Unknown abandoning outbox message error");
  }

  [TestMethod]
  public async Task abandoning__abandon_outbox_message__error_when_message_null()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var data = new AbandoningTestData { OutboxMessage = null };

    var (resultData, state, exception) = await AbandonOutboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandoningError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Outbox message is required.");
  }

  [TestMethod]
  public async Task abandoning__abandon_outbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-key-3",
      Payload = "payload-3",
      CreatedAt = DateTime.UtcNow
    };

    services.UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new AbandoningTestData { OutboxMessage = message };

    var (resultData, state, exception) = await AbandonOutboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task abandoning__abandon_outbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-key-4",
      Payload = "payload-4",
      CreatedAt = DateTime.UtcNow
    };

    var expectedException = new InvalidOperationException("DB failure");
    services.UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new AbandoningTestData { OutboxMessage = message };

    var (resultData, state, exception) = await AbandonOutboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandoningError);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task abandoning__abandon_outbox_message__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-key-5",
      Payload = "payload-5",
      CreatedAt = DateTime.UtcNow
    };

    var data = new AbandoningTestData { OutboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await AbandonOutboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), ct);
  }
}
