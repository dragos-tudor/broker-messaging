global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Shouldly;
global using NSubstitute;
global using AutoFixture;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Routing.Outbound;

[TestClass]
public partial class OutboundTests;

[TestClass]
public partial class IntegrationTests;
