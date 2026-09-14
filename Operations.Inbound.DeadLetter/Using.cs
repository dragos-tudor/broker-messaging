
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
global using static Operations.Inbound.DeadLetter.AbandoningStates;
global using static Operations.Inbound.DeadLetter.ClosingStates;
global using static Operations.Inbound.DeadLetter.InsertingStates;
global using static Operations.Inbound.DeadLetter.MappingStates;
global using static Operations.Inbound.DeadLetter.SchedulingStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetter;

public static partial class DeadLetterFuncs;
