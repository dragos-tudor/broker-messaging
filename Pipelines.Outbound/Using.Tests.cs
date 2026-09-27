
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using AutoFixture;
global using Shouldly;

namespace Pipelines.Outbound;

[TestClass]
public partial class OutboundTests
{
  static readonly IFixture Fixture = new Fixture();
}
