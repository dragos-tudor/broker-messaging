
global using System;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.InboxMessage;
global using Transport.Envelope;
global using Transport.DeadLetterEnvelope;
global using static Persistence.InboxMessage.InboxMessageFuncs;
global using static Transport.Envelope.EnvelopeFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.Envelope;

public static partial class EnvelopeFuncs;