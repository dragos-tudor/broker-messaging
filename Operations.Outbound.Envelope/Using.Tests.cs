
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using static Operations.Outbound.Envelope.EnvelopeFuncs;
global using static Operations.Outbound.Envelope.EnvelopeStates;

namespace Operations.Outbound.Envelope;

[TestClass]
public partial class EnvelopeTests;