
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using static Foundation.Extensions.ExtensionsFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Outbound")]

namespace Operations.Outbound.Envelope;

public static partial class EnvelopeFuncs;
