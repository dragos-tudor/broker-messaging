
namespace Kafka.Clients;

partial class ClientsFuncs
{
  const char EndpointSeparator = ',';

  public static IEnumerable<string> SplitKafkaEndpoints(string? endpoints)
      => string.IsNullOrWhiteSpace(endpoints)
        ? []
        : endpoints.Split(EndpointSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}