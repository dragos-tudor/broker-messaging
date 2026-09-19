set -e

CONFIGURATION=${1:-Debug}

cd $WORKSPACE_ROOT
dotnet test --solution messaging.core.slnx \
  --configuration $CONFIGURATION \
  --no-restore \
  --no-build \
  --verbosity minimal

 dotnet test --solution messaging.kafka.slnx \
  --configuration $CONFIGURATION \
  --no-restore \
  --no-build \
  --verbosity minimal



