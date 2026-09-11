
namespace Operations.Inbound.Inbox;

public class DomainException : Exception
{
  public DomainException() : base() { }
  public DomainException(string message, Exception innerException) : base(message, innerException) { }
  public DomainException(string message) : base(message) { }
}

partial class InboxFuncs
{
  internal static DomainException CreateDomainException(string error) =>
    new (error);
}