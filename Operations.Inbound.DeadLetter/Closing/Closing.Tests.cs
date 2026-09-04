namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  sealed class ClosingTestData : IClosingData<string, string>
  {
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
  }

  [TestMethod]
  public async Task closing__close_dead_letter_message__success_when_message_updated()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-close-1",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new ClosingTestData { DeadLetterMessage = message };

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await services.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await CloseDeadLetterMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, default);

    state.ShouldBe(ClosingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(DeadLetterMessageStatus.Published);
  }

  [TestMethod]
  public async Task closing__close_dead_letter_message__error_when_message_null()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var data = new ClosingTestData { DeadLetterMessage = null };

    var (resultData, state, exception) = await CloseDeadLetterMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, default);

    state.ShouldBe(ClosingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Dead letter message is required.");
  }

  [TestMethod]
  public async Task closing__close_dead_letter_message__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-close-2",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    services.UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(new OperationCanceledException());

    var data = new ClosingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await CloseDeadLetterMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, default);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task closing__close_dead_letter_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-close-3",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var expectedException = new InvalidOperationException("Failed closing update");
    services.UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new ClosingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await CloseDeadLetterMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, default);

    state.ShouldBe(ClosingError);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task closing__close_dead_letter_message__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-close-4",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new ClosingTestData { DeadLetterMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CloseDeadLetterMessageAsync<IClosingServices<string, string>, ClosingTestData, string, string>(services, data, ct);

    await services.Received(1).UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), ct);
  }
}

