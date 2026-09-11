
global using System;
global using Confluent.Kafka;
global using Transport.Envelope;
global using Transport.DeadLetterEnvelope;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Kafka.Services")]

namespace Kafka.Envelopes;

public static partial class EnvelopesFuncs;