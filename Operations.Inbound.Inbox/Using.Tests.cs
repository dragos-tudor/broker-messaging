

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using static Operations.Inbound.Inbox.InboxFuncs;
global using static Operations.Inbound.Inbox.InboxStates;

namespace Operations.Inbound.Inbox;

[TestClass]
public partial class InboxTests;