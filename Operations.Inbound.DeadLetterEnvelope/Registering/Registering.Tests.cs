namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  sealed class RegisteringTestData : IRegisteringRetryData<string, string, string, string>
  {
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public RetryPlan? RetryPlan { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__success_with_existing_retry_plan()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryPlanOptions();

    services.GetRetryPlanOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-key");
    envelope.CreatedAt.Returns(fixedDate.AddMinutes(-5));

    var existingRetry = new RetryPlan
    {
      RetryId = "retry-id-existing",
      RetryCount = 1
    };

    var data = new RegisteringTestData
    {
      DeadLetterEnvelope = envelope,
      RetryPlan = existingRetry,
      PipelineError = "Custom error"
    };

    var (resultData, state, exception) = await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data);

    state.ShouldBe(RegisterRetryDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).ScheduleRetryPlanAsync(existingRetry, Arg.Any<Func<RetryPlan, RetryPlan>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__success_creating_new_retry_plan()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryPlanOptions();

    services.GetRetryPlanOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-new");
    envelope.CreatedAt.Returns(fixedDate.AddMinutes(-5));

    var data = new RegisteringTestData
    {
      DeadLetterEnvelope = envelope,
      RetryPlan = null,
      PipelineError = "Error msg"
    };

    var (resultData, state, exception) = await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data);

    state.ShouldBe(RegisterRetryDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).ScheduleRetryPlanAsync(
      Arg.Is<RetryPlan>(r => r.RetryId == BuildRetryPlanId(envelope.Key, envelope.CreatedAt)),
      Arg.Any<Func<RetryPlan, RetryPlan>>(),
      Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__uses_default_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var options = new RetryPlanOptions();

    services.GetRetryPlanOptions().Returns(options);
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-def");
    envelope.CreatedAt.Returns(fixedDate);

    var data = new RegisteringTestData
    {
      DeadLetterEnvelope = envelope,
      PipelineError = null!
    };

    Func<RetryPlan, RetryPlan>? capturedUpdate = null;
    await services.ScheduleRetryPlanAsync(Arg.Any<RetryPlan>(), Arg.Do<Func<RetryPlan, RetryPlan>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var (resultData, state, exception) = await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data);

    state.ShouldBe(RegisterRetryDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    capturedUpdate.ShouldNotBeNull();
    var dummyRetry = new RetryPlan { RetryId = "dummy" };
    var updated = capturedUpdate(dummyRetry);
    updated.LastError.ShouldBe("Unknown register retry dead letter envelope error");
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var data = new RegisteringTestData { DeadLetterEnvelope = null };

    var (resultData, state, exception) = await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data);

    state.ShouldBe(RegisterRetryDeadLetterEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter envelope is required.");
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-cancel");
    envelope.CreatedAt.Returns(fixedDate);

    services.ScheduleRetryPlanAsync(Arg.Any<RetryPlan>(), Arg.Any<Func<RetryPlan, RetryPlan>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new RegisteringTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var expectedException = new InvalidOperationException("Schedule DB failure");
    services.GetRetryPlanOptions().Throws(expectedException);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-err");
    envelope.CreatedAt.Returns(fixedDate);

    var data = new RegisteringTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data);

    state.ShouldBe(RegisterRetryDeadLetterEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Schedule DB failure");
  }

  [TestMethod]
  public async Task registering__register_retry_dead_letter_envelope__cancellation_token_forwarded()
  {
    var services = Substitute.For<IRegisteringRetryServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions());
    services.GetUtcDateTime().Returns(fixedDate);

    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    envelope.Key.Returns("dl-upsert-token");
    envelope.CreatedAt.Returns(fixedDate);

    var data = new RegisteringTestData { DeadLetterEnvelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await RegisterRetryDeadLetterEnvelopeAsync<IRegisteringRetryServices, RegisteringTestData, string, string, string, string>(services, data, ct);

    await services.Received(1).ScheduleRetryPlanAsync(Arg.Any<RetryPlan>(), Arg.Any<Func<RetryPlan, RetryPlan>>(), ct);
  }
}
