namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class UpsertingTestData : IUpsertingRetryData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public RetryMessage? RetryMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__success_with_existing_retry_message()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryMessageOptions();

    services.GetRetryMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate.AddMinutes(-5)
    };
    var existingRetry = new RetryMessage
    {
      RetryId = "retry-id-1",
      RetryCount = 1
    };

    var data = new UpsertingTestData
    {
      InboxMessage = message,
      RetryMessage = existingRetry,
      PipelineError = "Custom error"
    };

    var (resultData, state, exception) = await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data);

    state.ShouldBe(UpsertRetryInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).UpsertRetryMessageAsync(existingRetry, Arg.Any<Func<RetryMessage, RetryMessage>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__success_creating_new_retry_message()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryMessageOptions();

    services.GetRetryMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key-new",
      Payload = "payload",
      CreatedAt = fixedDate.AddMinutes(-5)
    };

    var data = new UpsertingTestData
    {
      InboxMessage = message,
      RetryMessage = null,
      PipelineError = "Failure occurred"
    };

    var (resultData, state, exception) = await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data);

    state.ShouldBe(UpsertRetryInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).UpsertRetryMessageAsync(
      Arg.Is<RetryMessage>(r => r.RetryId == BuildRetryMessageId(message.MessageKey, message.CreatedAt)),
      Arg.Any<Func<RetryMessage, RetryMessage>>(),
      Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryMessageOptions();

    services.GetRetryMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };

    var data = new UpsertingTestData
    {
      InboxMessage = message,
      PipelineError = null!
    };

    Func<RetryMessage, RetryMessage>? capturedUpdate = null;
    await services.UpsertRetryMessageAsync(Arg.Any<RetryMessage>(), Arg.Do<Func<RetryMessage, RetryMessage>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data);

    state.ShouldBe(UpsertRetryInboxMessageSuccessState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var dummyRetry = new RetryMessage { RetryId = "dummy" };
    var updated = capturedUpdate(dummyRetry);
    updated.LastError.ShouldBe("Unknown upsert retry inbox message error");
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var data = new UpsertingTestData { InboxMessage = null };

    var (resultData, state, exception) = await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data);

    state.ShouldBe(UpsertRetryInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };
    services.UpsertRetryMessageAsync(Arg.Any<RetryMessage>(), Arg.Any<Func<RetryMessage, RetryMessage>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new UpsertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var expectedException = new InvalidOperationException("Upsert DB failure");
    services.GetRetryMessageOptions().Throws(expectedException);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };
    var data = new UpsertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data);

    state.ShouldBe(UpsertRetryInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Upsert DB failure");
  }

  [TestMethod]
  public async Task upserting__upsert_retry_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<IUpsertingRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };
    var data = new UpsertingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await UpsertRetryInboxMessageAsync<IUpsertingRetryServices, UpsertingTestData, string, string>(services, data, ct);

    await services.Received(1).UpsertRetryMessageAsync(Arg.Any<RetryMessage>(), Arg.Any<Func<RetryMessage, RetryMessage>>(), ct);
  }
}

