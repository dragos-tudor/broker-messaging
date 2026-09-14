
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.InboxMessage;
global using Transport.Envelope;
global using Transport.DeadLetterEnvelope;
global using static Persistence.InboxMessage.InboxMessageFuncs;
global using static Transport.Envelope.EnvelopeFuncs;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Operations.Inbound.Envelope.CapturingStates;
global using static Operations.Inbound.Envelope.ConfirmingStates;
global using static Operations.Inbound.Envelope.ConfirmingFinalStates;
global using static Operations.Inbound.Envelope.ConvertingStates;
global using static Operations.Inbound.Envelope.MappingStates;
global using static Operations.Inbound.Envelope.VerifyingStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.Envelope;

public static partial class EnvelopeFuncs;
