namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class ClosingTestData : IClosingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task closing__close_inbox_message__success_when_message_updated()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "close-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var data = new ClosingTestData { InboxMessage = message };

    Func<InboxMessage<string, string>, InboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateInboxMessageAsync(message, Arg.Do<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await CloseInboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    state.ShouldBe(ClosingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(InboxMessageStatus.Closed);
  }

  [TestMethod]
  public async Task closing__close_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var data = new ClosingTestData { InboxMessage = null };

    var (resultData, state, exception) = await CloseInboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    state.ShouldBe(ClosingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task closing__close_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "close-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new ClosingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CloseInboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task closing__close_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "close-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var expectedException = new InvalidOperationException("Failed update");
    services.UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new ClosingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CloseInboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data);

    state.ShouldBe(ClosingError);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task closing__close_inbox_message__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "close-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var data = new ClosingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CloseInboxMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), ct);
  }
}
