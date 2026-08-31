
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using static Operations.Outbound.Outbox.OutboxFuncs;
global using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

[TestClass]
public partial class OutboxTests;