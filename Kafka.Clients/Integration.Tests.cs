
using System.Diagnostics;
using System.IO;

namespace Kafka.Clients;

public sealed partial class ClientsTests
{
  [TestMethod]
  public async Task messages__produce_messages__messages_consumed()
  {
    using var producer = CreateKafkaProducer<string, byte[]>(options);
    using var consumer = CreateKafkaConsumer<string, byte[]>(options);
    var producedKeys = new List<string>();
    var notProducedKeys = new List<string>();
    var consumedKeys = new List<string>();
    var logs = new List<string>();

    var stopWatch = new Stopwatch();
    stopWatch.Start();

    logs.Add($"before subscribe to topic. time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
    SubscribeConsumerToTopic(consumer, topicName);
    logs.Add($"after subscribe to topic. time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

    foreach (var index in Enumerable.Range(1, 500).Select(i => i))
    {
      var payload = new TestMessage(index, $"test-{index}");
      var message = CreateKafkaMessage($"test-key-{index}", JsonSerializer.SerializeToUtf8Bytes(payload), []);

      ProduceMessage(producer, topicName, message,
        report => {
          if (report.Status == PersistenceStatus.Persisted)
            producedKeys.Add(report.Message.Key);
          if (report.Status != PersistenceStatus.Persisted)
            notProducedKeys.Add(report.Message.Key);
        });
    }

    logs.Add($"before producer flush. time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
    producer.Flush(cancellationToken);
    logs.Add($"after producer flush. time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

    while(consumedKeys.Count < 500 && !cancellationToken.IsCancellationRequested) {
      var consumeResult = ConsumeMessage(consumer, cancellationToken);
      if (consumeResult == null || consumeResult.IsPartitionEOF) continue;
      consumedKeys.Add(consumeResult.Message.Key);
      // logs.Add($"Consumed message with key: {consumeResult.Message.Key}, time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
      OffsetConsumer(consumer, consumeResult, options);
    }
    CommitConsumerOffsets(consumer);
    UnsubscribeConsumer(consumer);

    stopWatch.Stop();
    logs.Add($"producedKeys.Count: {producedKeys.Count}");
    logs.Add($"notProducedKeys.Count: {notProducedKeys.Count}");
    logs.Add($"consumedKeys.Count: {consumedKeys.Count}");
    logs.Add($"consuming completed in {stopWatch.ElapsedMilliseconds} ms");

    string filePath = Path.Combine("/workspaces/broker-messaging/Kafka.Clients", $"kafka-tests-{DateTime.Now:yyyyMMdd-HHmmss}.log");
    await File.WriteAllTextAsync(filePath, string.Join(Environment.NewLine, logs), cancellationToken);

    // Assert.AreEqual(500, producedKeys.Count);
    // Assert.AreEqual(0, notProducedKeys.Count);
    // Assert.AreEqual(500, consumedKeys.Count);

    // Assert.AreEqual(500, producedKeys.Distinct().Count());
    // Assert.AreEqual(500, consumedKeys.Distinct().Count());
  }
}