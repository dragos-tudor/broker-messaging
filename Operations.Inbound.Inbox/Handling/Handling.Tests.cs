namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class HandlingTestData : IHandlingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public object? Model { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task handling__handle_inbox_message__success_when_handled_without_domain_error()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "handle-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var expectedModel = new object();
    services.HandleInboxMessageAsync(message, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<(object?, string?)>((expectedModel, null)));

    var data = new HandlingTestData { InboxMessage = message };

    var (resultData, state, exception) = await HandleInboxMessageAsync<IHandlingServices<string, string>, HandlingTestData, string, string>(services, data);

    state.ShouldBe(HandleInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.Model.ShouldBeSameAs(expectedModel);
  }

  [TestMethod]
  public async Task handling__handle_inbox_message__domain_error_when_handler_returns_domain_error()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "handle-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.HandleInboxMessageAsync(message, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<(object?, string?)>((null, "Item out of stock")));

    var data = new HandlingTestData { InboxMessage = message };

    var (resultData, state, exception) = await HandleInboxMessageAsync<IHandlingServices<string, string>, HandlingTestData, string, string>(services, data);

    state.ShouldBe(HandleInboxMessageDomainErrorState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldBe("Item out of stock");
    resultData.PipelineError.ShouldBe("Item out of stock");
  }

  [TestMethod]
  public async Task handling__handle_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var data = new HandlingTestData { InboxMessage = null };

    var (resultData, state, exception) = await HandleInboxMessageAsync<IHandlingServices<string, string>, HandlingTestData, string, string>(services, data);

    state.ShouldBe(HandleInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }

  [TestMethod]
  public async Task handling__handle_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "handle-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.HandleInboxMessageAsync(message, Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new HandlingTestData { InboxMessage = message };

    var (resultData, state, exception) = await HandleInboxMessageAsync<IHandlingServices<string, string>, HandlingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task handling__handle_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "handle-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var expectedException = new InvalidOperationException("Handler error");
    services.HandleInboxMessageAsync(message, Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);

    var data = new HandlingTestData { InboxMessage = message };

    var (resultData, state, exception) = await HandleInboxMessageAsync<IHandlingServices<string, string>, HandlingTestData, string, string>(services, data);

    state.ShouldBe(HandleInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Handler error");
  }

  [TestMethod]
  public async Task handling__handle_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<IHandlingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "handle-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var expectedModel = new object();
    services.HandleInboxMessageAsync(message, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<(object?, string?)>((expectedModel, null)));

    var data = new HandlingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await HandleInboxMessageAsync<IHandlingServices<string, string>, HandlingTestData, string, string>(services, data, ct);

    await services.Received(1).HandleInboxMessageAsync(message, ct);
  }
}

