namespace Kafka.Clients;

partial class ClientsFuncs
{
    public static string? GetKafkaUserName(string userName = "KAFKA_USERNAME") => Environment.GetEnvironmentVariable(userName);

    public static string? GetKafkaPassword(string password = "KAFKA_PASSWORD") => Environment.GetEnvironmentVariable(password);
}