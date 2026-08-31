namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  sealed class InsertingTestData : IInsertingData<string, string>
  {
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
  }

  sealed class FakeInsertingServices : IInsertingServices<string, string>
  {
    public Func<DeadLetterMessage<string, string>, CancellationToken, Task<bool>>? Handler { get; set; }
    public DeadLetterMessage<string, string>? ReceivedMessage { get; private set; }
    public CancellationToken ReceivedCt { get; private set; }
    public int CallCount { get; private set; }

    Task<bool> IDeadLetterMessageInsertService<string, string>.InsertDeadLetterMessageAsync(
      DeadLetterMessage<string, string> message,
      CancellationToken ct)
    {
      CallCount++;
      ReceivedMessage = message;
      ReceivedCt = ct;
      return Handler != null ? Handler(message, ct) : Task.FromResult(true);
    }
  }

  [TestMethod]
  public async Task inserting__insert_dead_letter_message__success_when_inserted()
  {
    var services = new FakeInsertingServices
    {
      Handler = (_, _) => Task.FromResult(true)
    };
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-insert-1",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new InsertingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await InsertDeadLetterMessageAsync<FakeInsertingServices, InsertingTestData, string, string>(services, data);

    state.ShouldBe(InsertDeadLetterMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    services.CallCount.ShouldBe(1);
    services.ReceivedMessage.ShouldBeSameAs(message);
  }

  [TestMethod]
  public async Task inserting__insert_dead_letter_message__idempotent_when_not_inserted()
  {
    var services = new FakeInsertingServices
    {
      Handler = (_, _) => Task.FromResult(false)
    };
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-insert-dup",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new InsertingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await InsertDeadLetterMessageAsync<FakeInsertingServices, InsertingTestData, string, string>(services, data);

    state.ShouldBe(IdempotentDeadLetterMessageState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    services.CallCount.ShouldBe(1);
  }

  [TestMethod]
  public async Task inserting__insert_dead_letter_message__error_when_message_null()
  {
    var services = new FakeInsertingServices();
    var data = new InsertingTestData { DeadLetterMessage = null };

    var (resultData, state, exception) = await InsertDeadLetterMessageAsync<FakeInsertingServices, InsertingTestData, string, string>(services, data);

    state.ShouldBe(InsertDeadLetterMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Dead letter message is required.");
    services.CallCount.ShouldBe(0);
  }

  [TestMethod]
  public async Task inserting__insert_dead_letter_message__returns_default_when_operation_canceled()
  {
    var services = new FakeInsertingServices
    {
      Handler = (_, _) => Task.FromException<bool>(new OperationCanceledException())
    };
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-insert-2",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new InsertingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await InsertDeadLetterMessageAsync<FakeInsertingServices, InsertingTestData, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task inserting__insert_dead_letter_message__error_when_service_throws_exception()
  {
    var expectedException = new InvalidOperationException("Insert DB failure");
    var services = new FakeInsertingServices
    {
      Handler = (_, _) => Task.FromException<bool>(expectedException)
    };
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-insert-3",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new InsertingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await InsertDeadLetterMessageAsync<FakeInsertingServices, InsertingTestData, string, string>(services, data);

    state.ShouldBe(InsertDeadLetterMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task inserting__insert_dead_letter_message__cancellation_token_forwarded_to_service()
  {
    var services = new FakeInsertingServices
    {
      Handler = (_, _) => Task.FromResult(true)
    };
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-insert-4",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };
    var data = new InsertingTestData { DeadLetterMessage = message };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await InsertDeadLetterMessageAsync<FakeInsertingServices, InsertingTestData, string, string>(services, data, ct);

    services.CallCount.ShouldBe(1);
    services.ReceivedCt.ShouldBe(ct);
  }
}

