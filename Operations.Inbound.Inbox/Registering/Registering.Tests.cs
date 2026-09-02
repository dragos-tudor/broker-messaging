namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class RegisteringTestData : IRegisteringRetryData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public RetryPlan? RetryPlan { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__success_with_existing_retry_plan()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryPlanOptions();

    services.GetRetryPlanOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate.AddMinutes(-5)
    };
    var existingRetry = new RetryPlan
    {
      RetryId = "retry-id-1",
      RetryCount = 1
    };

    var data = new RegisteringTestData
    {
      InboxMessage = message,
      RetryPlan = existingRetry,
      PipelineError = "Custom error"
    };

    var (resultData, state, exception) = await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data);

    state.ShouldBe(RegisterRetryInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).ScheduleRetryPlanAsync(existingRetry, Arg.Any<Func<RetryPlan, RetryPlan>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__success_creating_new_retry_plan()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryPlanOptions();

    services.GetRetryPlanOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key-new",
      Payload = "payload",
      CreatedAt = fixedDate.AddMinutes(-5)
    };

    var data = new RegisteringTestData
    {
      InboxMessage = message,
      RetryPlan = null,
      PipelineError = "Failure occurred"
    };

    var (resultData, state, exception) = await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data);

    state.ShouldBe(RegisterRetryInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).ScheduleRetryPlanAsync(
      Arg.Is<RetryPlan>(r => r.RetryId == BuildRetryPlanId(message.MessageKey, message.CreatedAt)),
      Arg.Any<Func<RetryPlan, RetryPlan>>(),
      Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryPlanOptions();

    services.GetRetryPlanOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };

    var data = new RegisteringTestData
    {
      InboxMessage = message,
      PipelineError = null!
    };

    Func<RetryPlan, RetryPlan>? capturedUpdate = null;
    await services.ScheduleRetryPlanAsync(Arg.Any<RetryPlan>(), Arg.Do<Func<RetryPlan, RetryPlan>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data);

    state.ShouldBe(RegisterRetryInboxMessageSuccessState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var dummyRetry = new RetryPlan { RetryId = "dummy" };
    var updated = capturedUpdate(dummyRetry);
    updated.LastError.ShouldBe("Unknown register retry inbox message error");
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var data = new RegisteringTestData { InboxMessage = null };

    var (resultData, state, exception) = await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data);

    state.ShouldBe(RegisterRetryInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };
    services.ScheduleRetryPlanAsync(Arg.Any<RetryPlan>(), Arg.Any<Func<RetryPlan, RetryPlan>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new RegisteringTestData { InboxMessage = message };

    var (resultData, state, exception) = await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var expectedException = new InvalidOperationException("Upsert DB failure");
    services.GetRetryPlanOptions().Throws(expectedException);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };
    var data = new RegisteringTestData { InboxMessage = message };

    var (resultData, state, exception) = await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data);

    state.ShouldBe(RegisterRetryInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Upsert DB failure");
  }

  [TestMethod]
  public async Task registering__register_retry_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "upsert-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };
    var data = new RegisteringTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await RegisterRetryInboxMessageAsync<IRegisteringRetryServices, RegisteringTestData, string, string>(services, data, ct);

    await services.Received(1).ScheduleRetryPlanAsync(Arg.Any<RetryPlan>(), Arg.Any<Func<RetryPlan, RetryPlan>>(), ct);
  }
}
