
namespace Kafka.Clients;

public abstract record ConnectionOptions
{
  public IEnumerable<string> EndPoints { get; init; } = [];
  public string User { get; init; } = default!;
  public string Password { get; init; } = default!;
  public TimeSpan? ConnectTimeout { get; init; } = default!;
  public bool Ssl { get; init; } = default!;
  public bool SslCertificateValidation { get; init; } = default!;
}