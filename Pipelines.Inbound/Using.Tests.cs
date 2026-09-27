
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Shouldly;
global using NSubstitute;
global using AutoFixture;
using AutoFixture.AutoNSubstitute;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Pipelines.Inbound;

[TestClass]
public partial class InboundTests
{
  static readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization()
  {
    ConfigureMembers = true,
    GenerateDelegates = true
  });
}