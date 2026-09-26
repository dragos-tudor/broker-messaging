
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using Foundation.Extensions;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
global using static Transport.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetter;

public static partial class DeadLetterFuncs;
