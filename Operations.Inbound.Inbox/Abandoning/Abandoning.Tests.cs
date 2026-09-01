namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class AbandoningTestData : IAbandoningData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task abandoning__abandon_inbox_message__success_when_message_updated()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "msg-key-1",
      Payload = "payload-1",
      CreatedAt = DateTime.UtcNow,
      Status = InboxMessageStatus.Processing
    };
    var data = new AbandoningTestData
    {
      InboxMessage = message,
      PipelineError = "Custom error"
    };

    var (resultData, state, exception) = await AbandonInboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandonInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task abandoning__abandon_inbox_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "msg-key-2",
      Payload = "payload-2",
      CreatedAt = DateTime.UtcNow,
      Status = InboxMessageStatus.Processing
    };
    var data = new AbandoningTestData
    {
      InboxMessage = message,
      PipelineError = null!
    };

    Func<InboxMessage<string, string>, InboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateInboxMessageAsync(message, Arg.Do<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await AbandonInboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandonInboxMessageSuccessState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(InboxMessageStatus.Abandoning);
    updated.LastError.ShouldBe("Unknown abandoning inbox message error.");
  }

  [TestMethod]
  public async Task abandoning__abandon_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var data = new AbandoningTestData { InboxMessage = null };

    var (resultData, state, exception) = await AbandonInboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandonInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task abandoning__abandon_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "msg-key-3",
      Payload = "payload-3",
      CreatedAt = DateTime.UtcNow
    };
    services.UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new AbandoningTestData { InboxMessage = message };

    var (resultData, state, exception) = await AbandonInboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task abandoning__abandon_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "msg-key-4",
      Payload = "payload-4",
      CreatedAt = DateTime.UtcNow
    };
    var expectedException = new InvalidOperationException("Update failed");
    services.UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new AbandoningTestData { InboxMessage = message };

    var (resultData, state, exception) = await AbandonInboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data);

    state.ShouldBe(AbandonInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task abandoning__abandon_inbox_message__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "msg-key-5",
      Payload = "payload-5",
      CreatedAt = DateTime.UtcNow
    };
    var data = new AbandoningTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await AbandonInboxMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), ct);
  }
}

