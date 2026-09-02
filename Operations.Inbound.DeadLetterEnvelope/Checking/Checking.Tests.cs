namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  sealed class CheckingTestData : ICheckingRetryData<string, string, string, string>
  {
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public RetryPlan? RetryPlan { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__not_exhausted_when_retry_message_is_null_and_max_attempts_greater_than_zero()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-1");
    envelope.CreatedAt.Returns(createdAt);

    var retryId = BuildRetryPlanId("dl-order-1", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryPlan?)null);

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CheckRetryDeadLetterEnvelopeNotExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__exhausted_when_retry_message_is_null_and_max_attempts_zero()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-1");
    envelope.CreatedAt.Returns(createdAt);

    var retryId = BuildRetryPlanId("dl-order-1", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 0 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryPlan?)null);

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CheckRetryDeadLetterEnvelopeExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__not_exhausted_when_attempts_remain()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-2");
    envelope.CreatedAt.Returns(createdAt);

    var retryId = BuildRetryPlanId("dl-order-2", createdAt);
    var retryPlan = new RetryPlan
    {
      RetryId = retryId,
      RetryCount = 2
    };

    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns(retryPlan);

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CheckRetryDeadLetterEnvelopeNotExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeSameAs(retryPlan);
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__exhausted_when_attempts_exceeded()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-3");
    envelope.CreatedAt.Returns(createdAt);

    var retryId = BuildRetryPlanId("dl-order-3", createdAt);
    var retryPlan = new RetryPlan
    {
      RetryId = retryId,
      RetryCount = 5
    };

    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns(retryPlan);

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CheckRetryDeadLetterEnvelopeExhaustedState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.RetryPlan.ShouldBeSameAs(retryPlan);
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var data = new CheckingTestData { DeadLetterEnvelope = null };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CheckRetryDeadLetterEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter envelope is required.");
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-4");
    envelope.CreatedAt.Returns(createdAt);

    var retryId = BuildRetryPlanId("dl-order-4", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Throws(new OperationCanceledException());

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-5");
    envelope.CreatedAt.Returns(createdAt);

    var expectedException = new InvalidOperationException("Storage error");
    services.GetRetryPlanOptions().Throws(expectedException);

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CheckRetryDeadLetterEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Storage error");
  }

  [TestMethod]
  public async Task checking__check_retry_dead_letter_envelope__cancellation_token_forwarded()
  {
    var services = Substitute.For<ICheckingRetryServices>();
    var createdAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-order-6");
    envelope.CreatedAt.Returns(createdAt);

    var retryId = BuildRetryPlanId("dl-order-6", createdAt);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 5 });
    services.GetRetryPlanByIdAsync(retryId, Arg.Any<CancellationToken>()).Returns((RetryPlan?)null);

    var data = new CheckingTestData { DeadLetterEnvelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CheckRetryDeadLetterEnvelopeAsync<ICheckingRetryServices, CheckingTestData, string, string, string, string>(services, data, ct);

    await services.Received(1).GetRetryPlanByIdAsync(retryId, ct);
  }
}

