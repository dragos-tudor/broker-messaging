
namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static string JoinKafkaEndpoints(IEnumerable<string> endpoints) =>
    string.Join(EndpointSeparator, endpoints.Where(endpoint => !string.IsNullOrWhiteSpace(endpoint)));
}