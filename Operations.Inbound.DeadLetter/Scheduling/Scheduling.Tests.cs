namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  sealed class SchedulingTestData : ISchedulingData<string, string>
  {
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__retry_state_when_attempts_remain()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new DeadLetterMessageOptions { MaxRetryAttempts = 5 };

    services.GetDeadLetterMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-sched-1",
      Payload = "payload",
      OriginatedAt = fixedDate.AddHours(-1),
      FailureReason = "initial error",
      RetryCount = 1
    };

    var data = new SchedulingTestData
    {
      DeadLetterMessage = message,
      PipelineError = "Publish error"
    };

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await services.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(SchedulingNotExhausted);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.RetryCount.ShouldBe(2);
    updated.Status.ShouldBe(DeadLetterMessageStatus.Processing);
    updated.LastError.ShouldBe("Publish error");
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__exhausted_state_when_attempts_exceeded()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new DeadLetterMessageOptions { MaxRetryAttempts = 3 };

    services.GetDeadLetterMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-sched-2",
      Payload = "payload",
      OriginatedAt = fixedDate.AddHours(-1),
      FailureReason = "initial error",
      RetryCount = 3
    };

    var data = new SchedulingTestData
    {
      DeadLetterMessage = message,
      PipelineError = "Publish error"
    };

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await services.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(SchedulingExhausted);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.RetryCount.ShouldBe(4);
    updated.Status.ShouldBe(DeadLetterMessageStatus.Abandoned);
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new DeadLetterMessageOptions { MaxRetryAttempts = 5 };

    services.GetDeadLetterMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-sched-3",
      Payload = "payload",
      OriginatedAt = fixedDate.AddHours(-1),
      FailureReason = "initial error",
      RetryCount = 0
    };

    var data = new SchedulingTestData
    {
      DeadLetterMessage = message,
      PipelineError = null!
    };

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await services.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(SchedulingNotExhausted);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.LastError.ShouldBe("Unknown scheduling dead letter message error");
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__error_when_message_null()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var data = new SchedulingTestData { DeadLetterMessage = null };

    var (resultData, state, exception) = await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(SchedulingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Dead letter message is required.");
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new DeadLetterMessageOptions { MaxRetryAttempts = 5 };

    services.GetDeadLetterMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-sched-4",
      Payload = "payload",
      OriginatedAt = fixedDate,
      FailureReason = "reason"
    };

    services.UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new SchedulingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    services.GetDeadLetterMessageOptions().Throws(new InvalidOperationException("Failed options"));

    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-sched-5",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new SchedulingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, default);

    state.ShouldBe(SchedulingError);
    exception.ShouldNotBeNull();
    exception.Message.ShouldBe("Failed options");
  }

  [TestMethod]
  public async Task scheduling__schedule_dead_letter_message__cancellation_token_forwarded()
  {
    var services = Substitute.For<ISchedulingServices<string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new DeadLetterMessageOptions { MaxRetryAttempts = 5 };

    services.GetDeadLetterMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-sched-6",
      Payload = "payload",
      OriginatedAt = fixedDate,
      FailureReason = "reason"
    };

    var data = new SchedulingTestData { DeadLetterMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await ScheduleDeadLetterMessageAsync<ISchedulingServices<string, string>, SchedulingTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), ct);
  }
}
