global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DispatchingStates;
global using static Operations.Inbound.DeadLetterEnvelope.ProducingStates;
global using static Operations.Inbound.DeadLetterEnvelope.PublishingStates;
global using static Operations.Inbound.DeadLetterEnvelope.RedirectingStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetterEnvelope;

public static partial class DeadLetterEnvelopeFuncs;
