set -e

CONFIGURATION=${1:-Debug}

cd $WORKSPACE_ROOT
dotnet build backend-kafka.slnx \
  --configuration $CONFIGURATION \
  --no-restore
