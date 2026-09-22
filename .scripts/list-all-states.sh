
for project in \
  Operations.Inbound.DeadLetter \
  Operations.Inbound.DeadLetterEnvelope \
  Operations.Inbound.Envelope \
  Operations.Inbound.Inbox \
  Operations.Outbound.Envelope \
  Operations.Outbound.Outbox
do
  $WORKSPACE_ROOT/.scripts/list-states.sh $WORKSPACE_ROOT/$project
done