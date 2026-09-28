global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Shouldly;
global using NSubstitute;
global using AutoFixture;
using AutoFixture.AutoNSubstitute;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Routing.Inbound;

[TestClass]
public partial class InboundTests;

[TestClass]
public partial class IntegrationTests;
