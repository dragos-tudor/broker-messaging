
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Operations.Inbound.DeadLetterEnvelope;

[TestClass]
public partial class DeadLetterEnvelopeTests;