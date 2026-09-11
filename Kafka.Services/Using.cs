
global using System;
global using System.Globalization;
global using System.Text;
global using Confluent.Kafka;
global using Persistence.DeadLetterMessage;
global using Persistence.InboxMessage;
global using Persistence.OutboxMessage;
global using Kafka.Envelopes;
global using Kafka.Messages;
global using Kafka.Services;
global using static Kafka.Envelopes.EnvelopesFuncs;
global using static Kafka.Messages.MessagesFuncs;
global using static Kafka.Services.ServicesFuncs;
global using static Persistence.InboxMessage.InboxMessageFuncs;

namespace Kafka.Services;

public static partial class ServicesFuncs;