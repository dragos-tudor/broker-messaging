namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class CheckingTestData : ICheckingRetryData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public RetryMessage? RetryMessage { get; set; }
    public string PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__not_exhausted_when_attempts_remain()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var retryId = BuildRetryMessageId("order-123", createdAt);
    var retryMessage = new RetryMessage
    {
      RetryId = retryId,
      RetryCount = 2
    };

    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 5 });
    services.GetRetryMessageByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns(retryMessage);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckRetryInboxMessageNotExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryMessage.ShouldBeSameAs(retryMessage);
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__exhausted_when_attempts_exceeded()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var retryId = BuildRetryMessageId("order-123", createdAt);
    var retryMessage = new RetryMessage
    {
      RetryId = retryId,
      RetryCount = 5
    };

    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 5 });
    services.GetRetryMessageByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns(retryMessage);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckRetryInboxMessageExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryMessage.ShouldBeSameAs(retryMessage);
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__not_exhausted_when_retry_message_is_null_and_max_attempts_greater_than_zero()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var retryId = BuildRetryMessageId("order-123", createdAt);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 5 });
    services.GetRetryMessageByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryMessage?)null);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckRetryInboxMessageNotExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryMessage.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__exhausted_when_retry_message_is_null_and_max_attempts_zero()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var retryId = BuildRetryMessageId("order-123", createdAt);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 0 });
    services.GetRetryMessageByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryMessage?)null);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckRetryInboxMessageExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryMessage.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var data = new CheckingTestData { InboxMessage = null };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckRetryInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var retryId = BuildRetryMessageId("order-123", createdAt);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 5 });
    services.GetRetryMessageByIdAsync(retryId, Arg.Any<CancellationToken>()).Throws(new OperationCanceledException());

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var expectedException = new InvalidOperationException("Storage failure");
    services.GetRetryMessageOptions().Throws(expectedException);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckRetryInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Storage failure");
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "order-123",
      Payload = "payload",
      CreatedAt = createdAt
    };

    var retryId = BuildRetryMessageId("order-123", createdAt);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 5 });
    services.GetRetryMessageByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryMessage?)null);

    var data = new CheckingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data, ct);

    await services.Received(1).GetRetryMessageByIdAsync(retryId, ct);
  }
}

