
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
global using static Operations.Inbound.DeadLetter.DeadLetterFuncs;
global using static Operations.Inbound.DeadLetter.DeadLetterStates;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetter;

public static partial class DeadLetterFuncs;