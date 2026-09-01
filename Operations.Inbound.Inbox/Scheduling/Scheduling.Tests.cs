namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class SchedulingTestData : ISchedulingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task scheduling__schedule_inbox_message__retry_state_when_attempts_remain()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new InboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetInboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "sched-key",
      Payload = "payload",
      RetryCount = 1,
      CreatedAt = fixedDate.AddMinutes(-5)
    };

    var data = new SchedulingTestData
    {
      InboxMessage = message,
      PipelineError = "Handler failure"
    };

    Func<InboxMessage<string, string>, InboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateInboxMessageAsync(message, Arg.Do<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleInboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(ScheduleInboxMessageRetryState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.RetryCount.ShouldBe(2);
    updated.Status.ShouldBe(InboxMessageStatus.Processing);
    updated.LastError.ShouldBe("Handler failure");
  }

  [TestMethod]
  public async Task scheduling__schedule_inbox_message__exhausted_state_when_attempts_exceeded()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new InboxMessageOptions { MaxRetryAttempts = 3 };

    services.GetInboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "sched-key-exhausted",
      Payload = "payload",
      RetryCount = 3,
      CreatedAt = fixedDate.AddMinutes(-5)
    };

    var data = new SchedulingTestData
    {
      InboxMessage = message,
      PipelineError = "Handler failure"
    };

    Func<InboxMessage<string, string>, InboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateInboxMessageAsync(message, Arg.Do<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleInboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(ScheduleInboxMessageExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.RetryCount.ShouldBe(4);
    updated.Status.ShouldBe(InboxMessageStatus.Abandoning);
  }

  [TestMethod]
  public async Task scheduling__schedule_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var data = new SchedulingTestData { InboxMessage = null };

    var (resultData, state, exception) = await ScheduleInboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(ScheduleInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task scheduling__schedule_inbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new InboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetInboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "sched-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };

    services.UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new SchedulingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ScheduleInboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task scheduling__schedule_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);

    services.GetInboxMessageOptions().Throws(new InvalidOperationException("Options failure"));

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "sched-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };

    var data = new SchedulingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ScheduleInboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(ScheduleInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldBe("Options failure");
  }

  [TestMethod]
  public async Task scheduling__schedule_inbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new InboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetInboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "sched-key",
      Payload = "payload",
      CreatedAt = fixedDate
    };

    var data = new SchedulingTestData { InboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await ScheduleInboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateInboxMessageAsync(message, Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(), ct);
  }
}

