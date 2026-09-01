
global using System;
global using System.Globalization;
global using System.Text;
global using Confluent.Kafka;
global using Persistence.InboxMessage;
global using Persistence.OutboxMessage;
global using Persistence.DeadLetterMessage;
global using Transport.Envelope;
global using Transport.DeadLetterEnvelope;
global using static Persistence.InboxMessage.InboxMessageFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
global using static Kafka.Messages.MessagesFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Kafka.Clients")]

namespace Kafka.Messages;

public static partial class MessagesFuncs;