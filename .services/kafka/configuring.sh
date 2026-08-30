set -euo pipefail
set +H

KAFKA_SERVER=${1:?missing kafka server}
KAFKA_SERVERS=${2:?missing kafka servers}

COUNT=0
MAX_RETRIES=5

echo "waiting for ${KAFKA_SERVER} to be ready ..."
until podman exec "${KAFKA_SERVER}" /opt/kafka/bin/kafka-broker-api-versions.sh \
  --bootstrap-server "${KAFKA_SERVERS}" >/dev/null 2>&1; do
  COUNT=$((COUNT + 1))
  if [ "${COUNT}" -ge "${MAX_RETRIES}" ]; then
    echo "Timed out waiting for ${KAFKA_SERVER} after $((MAX_RETRIES * 2))s"
    exit 1
  fi
  echo " still starting (${COUNT}/${MAX_RETRIES}) ..."
  sleep 2
done

echo "setting ${KAFKA_SERVER} password"
podman exec "${KAFKA_SERVER}" /opt/kafka/bin/kafka-configs.sh \
  --bootstrap-server "${KAFKA_SERVERS}" \
  --alter \
  --add-config "SCRAM-SHA-512=[password=${KAFKA_PASSWORD}]" \
  --entity-type users \
  --entity-name "${KAFKA_USERNAME}"