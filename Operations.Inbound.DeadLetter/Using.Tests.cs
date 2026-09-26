
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Operations.Inbound.DeadLetter;

[TestClass]
public partial class DeadLetterTests
{
  static object?[] CreateData<TKey, TPayload>(IDeadLetterMessage<TKey, TPayload>? message)
  {
    object?[] data = [null, null, null, null, null];
    if (message is not null)
      SetDeadLetterMessage(data, message);
    return data;
  }

  static readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization()
  {
    ConfigureMembers = true,
    GenerateDelegates = true
  });
}
