set -eu

VERSION=${1:?missing version}

cd $WORKSPACE_ROOT
for PROJECT in \
  "messaging.core/messaging.core.csproj" \
  "messaging.kafka/messaging.kafka.csproj"
do
  dotnet pack \
    --configuration Release \
    --output "${WORKSPACE_ROOT}/.packages" \
    -p:PackOnly=true \
    -p:Version="${VERSION}" \
    -p:PackageVersion="${VERSION}" \
    $PROJECT
done
