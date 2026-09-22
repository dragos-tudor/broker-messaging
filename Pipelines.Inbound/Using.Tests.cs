
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Shouldly;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Pipelines.Inbound;

[TestClass]
public partial class InboundTests;