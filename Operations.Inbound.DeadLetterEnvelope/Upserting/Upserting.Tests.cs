namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  sealed class UpsertingTestData : IUpsertingRetryData<string, string, string, string>
  {
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public RetryMessage? RetryMessage { get; set; }
    public string PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__success_with_existing_retry_message()
  {
    var services = Substitute.For<IUpsertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryMessageOptions();

    services.GetRetryMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-key");
    envelope.CreatedAt.Returns(fixedDate.AddMinutes(-5));

    var existingRetry = new RetryMessage
    {
      RetryId = "retry-id-existing",
      RetryCount = 1
    };

    var data = new UpsertingTestData
    {
      DeadLetterEnvelope = envelope,
      RetryMessage = existingRetry,
      PipelineError = "Custom error"
    };

    var (resultData, state, exception) = await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(UpsertRetryDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).UpsertRetryMessageAsync(existingRetry, Arg.Any<Func<RetryMessage, RetryMessage>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__success_creating_new_retry_message()
  {
    var services = Substitute.For<IUpsertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryMessageOptions();

    services.GetRetryMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-new");
    envelope.CreatedAt.Returns(fixedDate.AddMinutes(-5));

    var data = new UpsertingTestData
    {
      DeadLetterEnvelope = envelope,
      RetryMessage = null,
      PipelineError = "Error msg"
    };

    var (resultData, state, exception) = await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(UpsertRetryDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).UpsertRetryMessageAsync(
      Arg.Is<RetryMessage>(r => r.RetryId == BuildRetryMessageId(envelope.Key, envelope.CreatedAt)),
      Arg.Any<Func<RetryMessage, RetryMessage>>(),
      Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IUpsertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryMessageOptions();

    services.GetRetryMessageOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-def");
    envelope.CreatedAt.Returns(fixedDate);

    var data = new UpsertingTestData
    {
      DeadLetterEnvelope = envelope,
      PipelineError = null!
    };

    Func<RetryMessage, RetryMessage>? capturedUpdate = null;
    await services.UpsertRetryMessageAsync(Arg.Any<RetryMessage>(), Arg.Do<Func<RetryMessage, RetryMessage>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(UpsertRetryDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var dummyRetry = new RetryMessage { RetryId = "dummy" };
    var updated = capturedUpdate(dummyRetry);
    updated.LastError.ShouldBe("Unknown upsert retry inbox message error");
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IUpsertingServices>();
    var data = new UpsertingTestData { DeadLetterEnvelope = null };

    var (resultData, state, exception) = await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(UpsertRetryDeadLetterEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter envelope is required.");
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IUpsertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-cancel");
    envelope.CreatedAt.Returns(fixedDate);

    services.UpsertRetryMessageAsync(Arg.Any<RetryMessage>(), Arg.Any<Func<RetryMessage, RetryMessage>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new UpsertingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IUpsertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var expectedException = new InvalidOperationException("Upsert DB failure");
    services.GetRetryMessageOptions().Throws(expectedException);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-err");
    envelope.CreatedAt.Returns(fixedDate);

    var data = new UpsertingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(UpsertRetryDeadLetterEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Upsert DB failure");
  }

  [TestMethod]
  public async Task upserting__upsert_retry_dead_letter_envelope__cancellation_token_forwarded()
  {
    var services = Substitute.For<IUpsertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-token");
    envelope.CreatedAt.Returns(fixedDate);

    var data = new UpsertingTestData { DeadLetterEnvelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await UpsertRetryDeadLetterEnvelopeAsync<IUpsertingServices, UpsertingTestData, string, string, string, string>(services, data, ct);

    await services.Received(1).UpsertRetryMessageAsync(Arg.Any<RetryMessage>(), Arg.Any<Func<RetryMessage, RetryMessage>>(), ct);
  }
}

