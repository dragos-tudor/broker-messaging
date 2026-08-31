namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  sealed class SchedulingTestData : ISchedulingData<string, string>
  {
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__retry_state_when_attempts_remain()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new OutboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetOutboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-sched-1",
      Payload = "payload",
      CreatedAt = fixedDate.AddHours(-1),
      RetryCount = 1
    };

    var data = new SchedulingTestData
    {
      OutboxMessage = message,
      PipelineError = "Scheduling pipeline error"
    };

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data);

    state.ShouldBe(ScheduleOutboxMessageRetryState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.RetryCount.ShouldBe(2);
    updated.Status.ShouldBe(OutboxMessageStatus.Processing);
    updated.LastError.ShouldBe("Scheduling pipeline error");
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__exhausted_state_when_attempts_exceeded()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new OutboxMessageOptions { MaxRetryAttempts = 3 };

    services.GetOutboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-sched-2",
      Payload = "payload",
      CreatedAt = fixedDate.AddHours(-1),
      RetryCount = 3
    };

    var data = new SchedulingTestData
    {
      OutboxMessage = message,
      PipelineError = "Scheduling pipeline error"
    };

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data);

    state.ShouldBe(ScheduleOutboxMessageExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.RetryCount.ShouldBe(4);
    updated.Status.ShouldBe(OutboxMessageStatus.Abandoned);
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new OutboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetOutboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-sched-3",
      Payload = "payload",
      CreatedAt = fixedDate.AddHours(-1),
      RetryCount = 0
    };

    var data = new SchedulingTestData
    {
      OutboxMessage = message,
      PipelineError = null!
    };

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await services.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data);

    state.ShouldBe(ScheduleOutboxMessageRetryState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.LastError.ShouldBe("Unknown scheduling outbox message error.");
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__error_when_message_null()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var data = new SchedulingTestData { OutboxMessage = null };

    var (resultData, state, exception) = await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data);

    state.ShouldBe(ScheduleOutboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Outbox message is required.");
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new OutboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetOutboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-sched-4",
      Payload = "payload",
      CreatedAt = fixedDate,
      RetryCount = 0
    };

    services.UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new SchedulingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    services.GetOutboxMessageOptions().Throws(new InvalidOperationException("Failed options"));

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-sched-5",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow,
      RetryCount = 0
    };

    var data = new SchedulingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data);

    state.ShouldBe(ScheduleOutboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldBe("Failed options");
  }

  [TestMethod]
  public async Task scheduling__schedule_outbox_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new OutboxMessageOptions { MaxRetryAttempts = 5 };

    services.GetOutboxMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-sched-6",
      Payload = "payload",
      CreatedAt = fixedDate,
      RetryCount = 0
    };

    var data = new SchedulingTestData { OutboxMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await ScheduleOutboxMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string, string>(services, data, ct);

    await services.Received(1).UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), ct);
  }
}
