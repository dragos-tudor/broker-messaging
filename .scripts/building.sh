set -e

CONFIGURATION=${1:-Debug}

cd $WORKSPACE_ROOT
for SOLUTION in \
  messaging.core.slnx \
  messaging.kafka.slnx
do
  dotnet build "$SOLUTION" \
    --configuration "$CONFIGURATION" \
    --no-restore
done
