namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  sealed class AbandoningTestData : IAbandoningData<string, string>
  {
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
    public string PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task abandoning__abandon_dead_letter_message__success_when_message_updated()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-key-1",
      Payload = "payload-1",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "Initial error"
    };

    var data = new AbandoningTestData
    {
      DeadLetterMessage = message,
      PipelineError = "Abandoning error"
    };

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await services.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, default);

    state.ShouldBe(AbandonDeadLetterMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(DeadLetterMessageStatus.Abandoned);
    updated.LastError.ShouldBe("Abandoning error");
  }

  [TestMethod]
  public async Task abandoning__abandon_dead_letter_message__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-key-2",
      Payload = "payload-2",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "Initial error"
    };

    var data = new AbandoningTestData
    {
      DeadLetterMessage = message,
      PipelineError = null!
    };

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await services.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, default);

    state.ShouldBe(AbandonDeadLetterMessageSuccessState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(DeadLetterMessageStatus.Abandoned);
    updated.LastError.ShouldBe("Unknown abandoning dead letter message error");
  }

  [TestMethod]
  public async Task abandoning__abandon_dead_letter_message__error_when_message_null()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var data = new AbandoningTestData { DeadLetterMessage = null };

    var (resultData, state, exception) = await AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, default);

    state.ShouldBe(AbandonDeadLetterMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Dead letter message is required.");
  }

  [TestMethod]
  public async Task abandoning__abandon_dead_letter_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-key-3",
      Payload = "payload-3",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "Initial error"
    };

    services.UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new AbandoningTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, default);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task abandoning__abandon_dead_letter_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-key-4",
      Payload = "payload-4",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "Initial error"
    };

    var expectedException = new InvalidOperationException("DB error");
    services.UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new AbandoningTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, default);

    state.ShouldBe(AbandonDeadLetterMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task abandoning__abandon_dead_letter_message__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-key-5",
      Payload = "payload-5",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "Initial error"
    };

    var data = new AbandoningTestData { DeadLetterMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, AbandoningTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), ct);
  }
}

