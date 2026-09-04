namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class InsertingTestData : IInsertingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task inserting__insert_inbox_message__success_with_status_processing_when_inserted()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "insert-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.InsertInboxMessageAsync(message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

    var data = new InsertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await InsertInboxMessageAsync<IInsertingServices<string, string>, InsertingTestData, string, string>(services, data);

    state.ShouldBe(InsertingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.InboxMessage.ShouldBeSameAs(message);
    resultData.InboxMessage.ShouldBeEquivalentTo(new { Status = InboxMessageStatus.Processing });
  }

  [TestMethod]
  public async Task inserting__insert_inbox_message__idempotent_when_already_exists()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "insert-key-duplicate",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.InsertInboxMessageAsync(message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

    var data = new InsertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await InsertInboxMessageAsync<IInsertingServices<string, string>, InsertingTestData, string, string>(services, data);

    state.ShouldBe(Idempotent);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.InboxMessage.ShouldBeNull();
  }

  [TestMethod]
  public async Task inserting__insert_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var data = new InsertingTestData { InboxMessage = null };

    var (resultData, state, exception) = await InsertInboxMessageAsync<IInsertingServices<string, string>, InsertingTestData, string, string>(services, data);

    state.ShouldBe(InsertingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }

  [TestMethod]
  public async Task inserting__insert_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "insert-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.InsertInboxMessageAsync(message, Arg.Any<CancellationToken>()).ThrowsAsync(new OperationCanceledException());

    var data = new InsertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await InsertInboxMessageAsync<IInsertingServices<string, string>, InsertingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task inserting__insert_inbox_message__error_with_status_mapping_when_service_throws_exception()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "insert-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var expectedException = new InvalidOperationException("DB insert error");
    services.InsertInboxMessageAsync(message, Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);

    var data = new InsertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await InsertInboxMessageAsync<IInsertingServices<string, string>, InsertingTestData, string, string>(services, data);

    state.ShouldBe(InsertingError);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("DB insert error");
    resultData.InboxMessage.ShouldBeEquivalentTo(new { Status = InboxMessageStatus.Initial });
  }

  [TestMethod]
  public async Task inserting__insert_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "insert-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    services.InsertInboxMessageAsync(message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

    var data = new InsertingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await InsertInboxMessageAsync<IInsertingServices<string, string>, InsertingTestData, string, string>(services, data, ct);

    await services.Received(1).InsertInboxMessageAsync(message, ct);
  }
}
