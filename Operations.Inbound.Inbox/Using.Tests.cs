
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Operations.Inbound.Inbox;

[TestClass]
public partial class InboxTests
{
  static object?[] CreateInboxData<TKey, TPayload>(
    IInboxMessage<TKey, TPayload>? message = null,
    IDeadLetterMessage<TKey, TPayload>? deadLetterMessage = null,
    object? model = null)
  {
    object?[] data = new object?[6];
    if (message is not null)
      SetInboxMessage(data, message);
    if (deadLetterMessage is not null)
      SetDeadLetterMessage(data, deadLetterMessage);
    if (model is not null)
      InboxFuncs.SetDomainModel(data, model);
    return data;
  }

  static readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization()
  {
    ConfigureMembers = true,
    GenerateDelegates = true
  });
}