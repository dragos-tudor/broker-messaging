
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetter;

public static partial class DeadLetterFuncs;