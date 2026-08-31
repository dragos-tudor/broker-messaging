namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class ConvertingTestData : IConvertingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
  }

  [TestMethod]
  public async Task converting__convert_inbox_message__success_when_dead_letter_message_created()
  {
    var services = Substitute.For<IConvertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "convert-key",
      Payload = "payload",
      LastError = "Custom error",
      CreatedAt = fixedDate.AddMinutes(-5)
    };
    var data = new ConvertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ConvertInboxMessage<IConvertingServices, ConvertingTestData, string, string>(services, data);

    state.ShouldBe(ConvertInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.DeadLetterMessage.ShouldNotBeNull();
    resultData.DeadLetterMessage.MessageKey.ShouldBe("convert-key");
    resultData.DeadLetterMessage.FailureReason.ShouldBe("Custom error");
    resultData.DeadLetterMessage.CreatedAt.ShouldBe(fixedDate);
  }

  [TestMethod]
  public async Task converting__convert_inbox_message__uses_default_error_when_last_error_null()
  {
    var services = Substitute.For<IConvertingServices>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    services.GetUtcDateTime().Returns(fixedDate);

    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "convert-key-2",
      Payload = "payload",
      LastError = null,
      CreatedAt = fixedDate.AddMinutes(-5)
    };
    var data = new ConvertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ConvertInboxMessage<IConvertingServices, ConvertingTestData, string, string>(services, data);

    state.ShouldBe(ConvertInboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.DeadLetterMessage.ShouldNotBeNull();
    resultData.DeadLetterMessage.FailureReason.ShouldBe("Unknown converting inbox message error.");
  }

  [TestMethod]
  public async Task converting__convert_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IConvertingServices>();
    var data = new ConvertingTestData { InboxMessage = null };

    var (resultData, state, exception) = await ConvertInboxMessage<IConvertingServices, ConvertingTestData, string, string>(services, data);

    state.ShouldBe(ConvertInboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task converting__convert_inbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IConvertingServices>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "convert-key-3",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };
    var expectedException = new InvalidOperationException("Date service failed");
    services.GetUtcDateTime().Throws(expectedException);

    var data = new ConvertingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ConvertInboxMessage<IConvertingServices, ConvertingTestData, string, string>(services, data);

    state.ShouldBe(ConvertInboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public void converting__from_inbox_message__maps_all_fields_accurately()
  {
    var msgId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var originalCreatedAt = new DateTime(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);
    var deadLetterCreatedAt = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);

    var inboxMessage = new InboxMessage<string, string>
    {
      MessageId = msgId,
      MessageKey = "map-key",
      Payload = "map-payload",
      Type = "OrderCreated",
      Version = 2,
      Metadata = "meta-data",
      CorrelationId = correlationId,
      CreatedAt = originalCreatedAt
    };

    var dlMessage = FromInboxMessage(inboxMessage, "Reason", deadLetterCreatedAt);

    dlMessage.MessageId.ShouldBe(msgId);
    dlMessage.MessageKey.ShouldBe("map-key");
    dlMessage.Payload.ShouldBe("map-payload");
    dlMessage.Status.ShouldBe(DeadLetterMessageStatus.Processing);
    dlMessage.OriginatedAt.ShouldBe(originalCreatedAt);
    dlMessage.CreatedAt.ShouldBe(deadLetterCreatedAt);
    dlMessage.Type.ShouldBe("OrderCreated");
    dlMessage.Version.ShouldBe(2);
    dlMessage.Metadata.ShouldBe("meta-data");
    dlMessage.CorrelationId.ShouldBe(correlationId);
    dlMessage.FailureReason.ShouldBe("Reason");
  }
}

