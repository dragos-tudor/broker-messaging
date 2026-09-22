set -e

cd $WORKSPACE_ROOT
for SOLUTION in \
  messaging.core.slnx \
  messaging.kafka.slnx
do
  dotnet restore $SOLUTION
done