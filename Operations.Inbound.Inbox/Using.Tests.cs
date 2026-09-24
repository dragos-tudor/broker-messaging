
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
  static readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization()
  {
    ConfigureMembers = true,
    GenerateDelegates = true
  });
}