namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class CheckingTestData : ICheckingRetryData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public RetryPlan? RetryPlan { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
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

    var retryId = BuildRetryPlanId("order-123", createdAt);
    var retryPlan = new RetryPlan
    {
      RetryId = retryId,
      RetryCount = 2
    };

    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns(retryPlan);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckingRetryNotExhausted);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeSameAs(retryPlan);
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

    var retryId = BuildRetryPlanId("order-123", createdAt);
    var retryPlan = new RetryPlan
    {
      RetryId = retryId,
      RetryCount = 5
    };

    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns(retryPlan);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckingRetryExhausted);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeSameAs(retryPlan);
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

    var retryId = BuildRetryPlanId("order-123", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryPlan?)null);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckingRetryNotExhausted);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeNull();
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

    var retryId = BuildRetryPlanId("order-123", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 0 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryPlan?)null);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckingRetryExhausted);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var data = new CheckingTestData { InboxMessage = null };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckingRetryError);
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

    var retryId = BuildRetryPlanId("order-123", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Throws(new OperationCanceledException());

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
    services.GetRetryPlanOptions().Throws(expectedException);

    var data = new CheckingTestData { InboxMessage = message };

    var (resultData, state, exception) = await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data);

    state.ShouldBe(CheckingRetryError);
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

    var retryId = BuildRetryPlanId("order-123", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryPlan?)null);

    var data = new CheckingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CheckRetryInboxMessageAsync<ICheckingRetryServices, CheckingTestData, string, string>(services, data, ct);

    await services.Received(1).GetRetryPlanByIdAsync(retryId, ct);
  }
}
