

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using static Operations.Inbound.Envelope.EnvelopeFuncs;
global using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Operations.Inbound.Envelope;

[TestClass]
public partial class EnvelopeTests;