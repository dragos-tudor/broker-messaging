
global using AutoFixture;
global using AutoFixture.AutoNSubstitute;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;

namespace Operations.Inbound.Envelope;

[TestClass]
public partial class EnvelopeTests
{
  static readonly IFixture Fixture = new Fixture().Customize(
    new AutoNSubstituteCustomization
    {
      ConfigureMembers = true,
      GenerateDelegates = true
    });
}
