set -e

CONFIGURATION=${1:-Debug}

cd $WORKSPACE_ROOT
dotnet test --solution messaging-inboxoutbox.slnx \
  --configuration $CONFIGURATION \
  --no-restore \
  --no-build \
  --verbosity minimal



