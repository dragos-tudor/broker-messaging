
global using AutoFixture;
global using AutoFixture.AutoNSubstitute;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;

namespace Operations.Inbound.DeadLetterEnvelope;

[TestClass]
public partial class DeadLetterEnvelopeTests
{
  static readonly IFixture Fixture = new Fixture().Customize(
    new AutoNSubstituteCustomization
    {
      ConfigureMembers = true,
      GenerateDelegates = true
    });
}
