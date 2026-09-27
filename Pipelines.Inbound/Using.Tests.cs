
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Shouldly;
global using NSubstitute;
global using AutoFixture;

namespace Pipelines.Inbound;

[TestClass]
public partial class InboundTests
{
  static readonly IFixture Fixture = new Fixture();
}