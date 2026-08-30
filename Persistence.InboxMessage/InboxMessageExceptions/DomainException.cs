
namespace Persistence.InboxMessage;

public class DomainException : Exception
{
  public DomainException() : base() { }
  public DomainException(string message, Exception innerException) : base(message, innerException) { }
  public DomainException(string message) : base(message) { }
}
