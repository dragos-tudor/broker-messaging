set -eo pipefail
set +H

# dnf install -y bash-completion
# podman completion -f /etc/bash_completion.d/podman bash
# dotnet completions script bash

# podman run --rm docker.io/apache/kafka:4.3.1 /opt/kafka/bin/kafka-storage.sh random-uuid"
CLUSTER_ID=Y7fX2qL9RmKp0O4kS-VjnQ
KAFKA_SERVERS=kafka-1:9094,kafka-2:9094,kafka-3:9094
KAFKA_VOTERS=1@kafka-1:9093,2@kafka-2:9093,3@kafka-3:9093
SERVICES_ROOT=$WORKSPACE_ROOT/.services

"${SERVICES_ROOT}"/networks/removing.sh
"${SERVICES_ROOT}"/networks/creating.sh

"${SERVICES_ROOT}"/coredns/pulling.sh
"${SERVICES_ROOT}"/coredns/running.sh

"${SERVICES_ROOT}"/kafka/pulling.sh
"${SERVICES_ROOT}"/kafka/running.sh kafka-1 "${KAFKA_VOTERS}" 1 "$CLUSTER_ID"
"${SERVICES_ROOT}"/kafka/running.sh kafka-2 "${KAFKA_VOTERS}" 2 "$CLUSTER_ID"
"${SERVICES_ROOT}"/kafka/running.sh kafka-3 "${KAFKA_VOTERS}" 3 "$CLUSTER_ID"
"${SERVICES_ROOT}"/coredns/hosting.sh

"${SERVICES_ROOT}"/kafka/configuring.sh kafka-1 "${KAFKA_SERVERS}"
"${SERVICES_ROOT}"/kafka/configuring.sh kafka-2 "${KAFKA_SERVERS}"
"${SERVICES_ROOT}"/kafka/configuring.sh kafka-3 "${KAFKA_SERVERS}"
