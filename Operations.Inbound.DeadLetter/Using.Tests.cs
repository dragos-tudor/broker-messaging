
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using static Operations.Inbound.DeadLetter.DeadLetterFuncs;
global using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Operations.Inbound.DeadLetter;

[TestClass]
public partial class DeadLetterTests;